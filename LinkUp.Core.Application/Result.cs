namespace LinkUp.Core.Application
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Dictionary<string, List<string>>? Errors { get; }


        protected Result(bool isSuccess, Dictionary<string, List<string>>? errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public static Result Ok() => new(true, null);

        public static Result Fail(Dictionary<string, List<string>> errors) => new(false, errors);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        protected Result(bool isSuccess, T? value, Dictionary<string, List<string>>? error) : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Ok(T value) => new(true, value, null);

        public new static Result<T> Fail(Dictionary<string, List<string>> errors) => new(false, default, errors);
    }
}