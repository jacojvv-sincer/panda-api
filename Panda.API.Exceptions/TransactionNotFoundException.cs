using System;

namespace Panda.API.Exceptions
{
    public class TransactionNotFoundException : Exception
    {
        public TransactionNotFoundException() : base("Transaction does not exist")
        {
        }
    }
}