public class Authentication
{
    public Authentication()
    {
    }

    public bool Authenticate(string userName, string password)
    {
        if (userName == "lab74c" && password == "admccshlab74cadm")
            return true;
        return false;
    }
}