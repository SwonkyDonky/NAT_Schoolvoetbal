public class SessionManager
{
    private static Dictionary<int, User> userSessions = new Dictionary<int, User>();

    public static void AddSession(int sessionId, User user)
    {
        userSessions[sessionId] = user;
    }

    public static User GetSessionUser(int sessionId)
    {
        if (userSessions.ContainsKey(sessionId))
            return userSessions[sessionId];
        else
            return null;
    }

    public static void RemoveSession(int sessionId)
    {
        if (userSessions.ContainsKey(sessionId))
            userSessions.Remove(sessionId);
    }
}
