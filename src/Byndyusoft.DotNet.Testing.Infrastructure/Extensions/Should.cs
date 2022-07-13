namespace Byndyusoft.DotNet.Testing.Infrastructure.Extensions
{
    using System;
    using FluentAssertions;
    using FluentAssertions.Equivalency;
    using Moq;

    /// <summary>
    ///     Класс для использования FluentAssertions
    /// /// </summary>
    public static class Should
    {
        /// <summary>
        ///     Проверяет, что единственный вызов был вызван с похожими параметрами.
        ///     Если вызов не совпадает, то показывает разницу по полям.
        /// </summary>
        /// <remarks>
        ///     Использовать только в случае, когда ожидается единственный вызов метода.
        ///     Для валидации множественных вызовов использовать <see cref="AnyBeEquivalentTo{T}(T)" />
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="expected">ожидаемое значение параметра</param>
        /// <returns></returns>
        public static T BeEquivalentTo<T>(T expected)
        {
            return BeEquivalentTo(expected, options => options);
        }

        /// <summary>
        ///     Проверяет, что единственный вызов был вызван с похожими параметрами.
        ///     Если ни один вызов не совпадает, то показывается стандартное сообщение без разницы по полям.
        /// </summary>
        /// <remarks>
        ///     Использовать в случае, когда ожидается несколько вызовов метода.
        ///     Для валидации единственного вызова использовать <see cref="BeEquivalentTo{T}(T)" />
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="expected">ожидаемое значение параметра</param>
        /// <returns></returns>
        public static T AnyBeEquivalentTo<T>(T expected)
        {
            return AnyBeEquivalentTo(expected, options => options);
        }

        /// <summary>
        ///     Проверяет, что единственный вызов был вызван с похожими параметрами.
        ///     Если вызов не совпадает, то показывает разницу по полям.
        /// </summary>
        /// <remarks>
        ///     Использовать только в случае, когда ожидается единственный вызов метода.
        ///     Для валидации множественных вызовов использовать <see cref="AnyBeEquivalentTo{T}(T, Func{EquivalencyAssertionOptions, EquivalencyAssertionOptions{T}}" />
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="expected">ожидаемое значение параметра</param>
        /// <param name="options">опции сравнения</param>
        /// <returns></returns>
        public static T BeEquivalentTo<T>(T expected,
            Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> options)
        {
            return BeEquivalentTo<T>(actual => actual.Should().BeEquivalentTo(expected, options));
        }

        /// <summary>
        ///     Проверяет, что единственный вызов был вызван с похожими параметрами.
        ///     Если ни один вызов не совпадает, то показывается стандартное сообщение без разницы по полям.
        /// </summary>
        /// <remarks>
        ///     Использовать в случае, когда ожидается несколько вызовов метода.
        ///     Для валидации единственного вызова использовать <see cref="BeEquivalentTo{T}(T, Func{EquivalencyAssertionOptions{T}, EquivalencyAssertionOptions{T}}" />
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="expected">ожидаемое значение параметра</param>
        /// <param name="options">опции сравнения</param>
        /// <returns></returns>
        public static T AnyBeEquivalentTo<T>(T expected,
            Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> options)
        {
            return AnyBeEquivalentTo<T>(actual => actual.Should().BeEquivalentTo(expected, options));
        }

        /// <summary>
        ///     Проверяет, что единственный вызов был вызван с похожими параметрами.
        ///     Если вызов не совпадает, то показывает разницу по полям.
        /// </summary>
        /// <remarks>
        ///     Использовать только в случае, когда ожидается единственный вызов метода.
        ///     Для валидации множественных вызовов использовать <see cref="AnyBeEquivalentTo{T}(Action{T})" />
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="validateAction">экшен валидации</param>
        /// <returns></returns>
        public static T BeEquivalentTo<T>(Action<T> validateAction)
        {
            Predicate<T> validate = actual =>
            {
                validateAction(actual);
                return true;
            };

            return Match.Create(validate, () => BeEquivalentTo(validateAction));
        }

        /// <summary>
        ///     Проверяет, что единственный вызов был вызван с похожими параметрами.
        ///     Если ни один вызов не совпадает, то показывается стандартное сообщение без разницы по полям.
        /// </summary>
        /// <remarks>
        ///     Использовать в случае, когда ожидается несколько вызовов метода.
        ///     Для валидации единственного вызова использовать <see cref="BeEquivalentTo{T}(Action{T})" />
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="validateAction">экшен валидации</param>
        /// <returns></returns>
        public static T AnyBeEquivalentTo<T>(Action<T> validateAction)
        {
            Predicate<T> validate = actual =>
            {
                try
                {
                    validateAction(actual);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            };

            return Match.Create(validate, () => BeEquivalentTo(validateAction));
        }
    }
}