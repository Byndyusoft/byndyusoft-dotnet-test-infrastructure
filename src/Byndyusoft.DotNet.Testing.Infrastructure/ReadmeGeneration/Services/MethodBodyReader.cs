//
// MethodBaseRocks.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//
// (C) 2009 Novell, Inc. (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Services;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

internal sealed class Instruction
{

    int offset;
    OpCode opcode;
    object operand;

    Instruction previous;
    Instruction next;

    public int Offset
    {
        get { return offset; }
        set { offset = value; }
    }

    public OpCode OpCode
    {
        get { return opcode; }
        set { opcode = value; }
    }

    public object Operand
    {
        get { return operand; }
        set { operand = value; }
    }

    public Instruction? Previous
    {
        get { return previous; }
        set { previous = value; }
    }

    public Instruction Next
    {
        get { return next; }
        set { next = value; }
    }

    internal Instruction(int offset, OpCode opCode, object operand)
        : this(offset, opCode)
    {
        this.operand = operand;
    }

    internal Instruction(int offset, OpCode opCode)
    {
        this.offset = offset;
        this.opcode = opCode;
    }

    internal Instruction(OpCode opCode, object operand)
        : this(0, opCode, operand)
    {
    }

    internal Instruction(OpCode opCode)
        : this(0, opCode)
    {
    }

    public int GetSize()
    {
        int size = opcode.Size;

        switch (opcode.OperandType)
        {
            case OperandType.InlineSwitch:
                size += (1 + ((int[])operand).Length) * 4;
                break;
            case OperandType.InlineI8:
            case OperandType.InlineR:
                size += 8;
                break;
            case OperandType.InlineBrTarget:
            case OperandType.InlineField:
            case OperandType.InlineI:
            case OperandType.InlineMethod:
            case OperandType.InlineString:
            case OperandType.InlineTok:
            case OperandType.InlineType:
            case OperandType.ShortInlineR:
                size += 4;
                break;
            case OperandType.InlineVar:
                size += 2;
                break;
            case OperandType.ShortInlineBrTarget:
            case OperandType.ShortInlineI:
            case OperandType.ShortInlineVar:
                size += 1;
                break;
        }

        return size;
    }

    public override string ToString()
    {
        return opcode.Name;
    }
}

internal sealed class MethodBodyReader
{

    static OpCode[] one_byte_opcodes;
    static OpCode[] two_bytes_opcodes;

    static MethodBodyReader()
    {
        one_byte_opcodes = new OpCode[0xe1];
        two_bytes_opcodes = new OpCode[0x1f];

        foreach (var field in GetOpCodeFields())
        {
            var opcode = (OpCode)field.GetValue(null);
            if (opcode.OpCodeType == OpCodeType.Nternal)
                continue;

            if (opcode.Size == 1)
                one_byte_opcodes[opcode.Value] = opcode;
            else
                two_bytes_opcodes[opcode.Value & 0xff] = opcode;
        }
    }

    static IEnumerable<FieldInfo> GetOpCodeFields()
    {
        return typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static);
    }

    class ByteBuffer
    {

        internal byte[] buffer;
        internal int position;

        public ByteBuffer(byte[] buffer)
        {
            this.buffer = buffer;
        }

        public byte ReadByte()
        {
            CheckCanRead(1);
            return buffer[position++];
        }

        public short ReadInt16()
        {
            CheckCanRead(2);
            short @short = (short)(buffer[position]
                                   + (buffer[position + 1] << 8));
            position += 2;
            return @short;
        }

        public int ReadInt32()
        {
            CheckCanRead(4);
            int @int = buffer[position]
                       + (buffer[position + 1] << 8)
                       + (buffer[position + 2] << 16)
                       + (buffer[position + 3] << 24);
            position += 4;
            return @int;
        }

        public long ReadInt64()
        {
            CheckCanRead(8);
            long @long = buffer[position]
                         + (buffer[position + 1] << 8)
                         + (buffer[position + 2] << 16)
                         + (buffer[position + 3] << 24)
                         + (buffer[position + 4] << 32)
                         + (buffer[position + 5] << 40)
                         + (buffer[position + 6] << 48)
                         + (buffer[position + 7] << 56);
            position += 8;
            return @long;
        }

        public float ReadSingle()
        {
            CheckCanRead(4);
            float single = BitConverter.ToSingle(buffer, position);
            position += 4;
            return single;
        }

        public double ReadDouble()
        {
            CheckCanRead(8);
            double @double = BitConverter.ToDouble(buffer, position);
            position += 8;
            return @double;
        }

        void CheckCanRead(int count)
        {
            if (position + count > buffer.Length)
                throw new ArgumentOutOfRangeException();
        }
    }

    MethodBase method;
    MethodBody body;
    Module module;
    Type[] type_arguments;
    Type[] method_arguments;
    ByteBuffer raw_il;
    ParameterInfo[] parameters;
    IList<LocalVariableInfo> locals;
    List<Instruction> instructions = new List<Instruction>();
    Instruction instruction;

    MethodBodyReader(MethodBase method)
    {
        this.method = method;

        this.body = method.GetMethodBody();
        if (this.body == null)
            throw new ArgumentException();

        var bytes = body.GetILAsByteArray();
        if (bytes == null)
            throw new ArgumentException();

        if (!(method is ConstructorInfo))
            method_arguments = method.GetGenericArguments();

        if (method.DeclaringType != null)
            type_arguments = method.DeclaringType.GetGenericArguments();

        this.parameters = method.GetParameters();
        this.locals = body.LocalVariables;
        this.module = method.Module;
        this.raw_il = new ByteBuffer(bytes);
    }

    void ReadInstructions()
    {
        while (raw_il.position < raw_il.buffer.Length)
        {
            CreateInstruction();
            ReadInstruction();
            instructions.Add(instruction);
        }
    }

    void CreateInstruction()
    {
        var previous = instruction;
        instruction = new Instruction(raw_il.position, ReadOpCode());

        if (previous != null)
        {
            instruction.Previous = previous;
            previous.Next = instruction;
        }
    }

    void ReadInstruction()
    {
        switch (instruction.OpCode.OperandType)
        {
            case OperandType.InlineNone:
                break;
            case OperandType.InlineSwitch:
                int length = raw_il.ReadInt32();
                int[] branches = new int[length];
                int[] offsets = new int[length];
                for (int i = 0; i < length; i++)
                    offsets[i] = raw_il.ReadInt32();
                for (int i = 0; i < length; i++)
                    branches[i] = raw_il.position + offsets[i];

                instruction.Operand = branches;
                break;
            case OperandType.ShortInlineBrTarget:
                instruction.Operand = raw_il.position - (sbyte)raw_il.ReadByte();
                break;
            case OperandType.InlineBrTarget:
                instruction.Operand = raw_il.position - raw_il.ReadInt32();
                break;
            case OperandType.ShortInlineI:
                if (instruction.OpCode == OpCodes.Ldc_I4_S)
                    instruction.Operand = (sbyte)raw_il.ReadByte();
                else
                    instruction.Operand = raw_il.ReadByte();
                break;
            case OperandType.InlineI:
                instruction.Operand = raw_il.ReadInt32();
                break;
            case OperandType.ShortInlineR:
                instruction.Operand = raw_il.ReadSingle();
                break;
            case OperandType.InlineR:
                instruction.Operand = raw_il.ReadDouble();
                break;
            case OperandType.InlineI8:
                instruction.Operand = raw_il.ReadInt64();
                break;
            case OperandType.InlineSig:
                instruction.Operand = module.ResolveSignature(raw_il.ReadInt32());
                break;
            case OperandType.InlineString:
                instruction.Operand = module.ResolveString(raw_il.ReadInt32());
                break;
            case OperandType.InlineTok:
                instruction.Operand = module.ResolveMember(raw_il.ReadInt32(), type_arguments, method_arguments);
                break;
            case OperandType.InlineType:
                instruction.Operand = module.ResolveType(raw_il.ReadInt32(), type_arguments, method_arguments);
                break;
            case OperandType.InlineMethod:
                instruction.Operand = module.ResolveMethod(raw_il.ReadInt32(), type_arguments, method_arguments);
                break;
            case OperandType.InlineField:
                instruction.Operand = module.ResolveField(raw_il.ReadInt32(), type_arguments, method_arguments);
                break;
            case OperandType.ShortInlineVar:
                instruction.Operand = GetVariable(raw_il.ReadByte());
                break;
            case OperandType.InlineVar:
                instruction.Operand = GetVariable(raw_il.ReadInt16());
                break;
            default:
                throw new NotSupportedException();
        }
    }

    object GetVariable(int index)
    {
        if (TargetsLocalVariable(instruction.OpCode))
            return GetLocalVariable(index);
        else
            return GetParameter(index);
    }

    static bool TargetsLocalVariable(OpCode opcode)
    {
        return opcode.Name.Contains("loc");
    }

    LocalVariableInfo GetLocalVariable(int index)
    {
        return locals[index];
    }

    ParameterInfo GetParameter(int index)
    {
        if (!method.IsStatic)
            index--;

        return parameters[index];
    }

    OpCode ReadOpCode()
    {
        byte il_opcode = raw_il.ReadByte();
        return il_opcode != 0xfe
                   ? one_byte_opcodes[il_opcode]
                   : two_bytes_opcodes[raw_il.ReadByte()];
    }

    public static List<Instruction> GetInstructions(MethodBase method)
    {
        var reader = new MethodBodyReader(method);
        reader.ReadInstructions();
        return reader.instructions;
    }
}

internal static class MethodBaseExtensions
{
    internal static IList<Instruction> GetInstructions(this MethodBase self)
    {
        return MethodBodyReader.GetInstructions(self).AsReadOnly();
    }
}