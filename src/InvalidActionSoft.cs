using System;

namespace LOCAM {

    public class InvalidActionSoft : Exception
    {
        //private static final long serialVersionUID = -8185589153224401564L;

        public InvalidActionSoft(string message):base(message)
        {
        }

    }
}