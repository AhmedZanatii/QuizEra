namespace QuizEra.BLL.ModelVM.Auth
{
    public enum LoginResult
    {
        Success,
        UserNotFound,
        Deactivated,
        InvalidPassword,
        NotAllowed,
        LockedOut
    }
}