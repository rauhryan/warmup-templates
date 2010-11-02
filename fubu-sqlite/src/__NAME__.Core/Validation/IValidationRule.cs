namespace __NAME__.Core.Validation
{
    public interface IValidationRule
    {
        string Message { get; set; }
        void Validate(object target, Notification notification);
    }
}