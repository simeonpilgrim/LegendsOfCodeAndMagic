using System;

namespace LOCAM
{
    public class InvalidActionHard : Exception
    {
       // private static final long serialVersionUID = -8185589153224401565L;

        public InvalidActionHard(string message, Exception e):base(message, e)
        {
        }
    }
}