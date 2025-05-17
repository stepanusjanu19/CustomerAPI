using System;

namespace Application.Exceptions
{
    public class DuplicateCustomerException : Exception
    {
        private string _customerCode;
        private string _customerName;

        public DuplicateCustomerException(string customerCode, string customerName)
            : base($"Customer with code '{customerCode}' already exists and is used by '{customerName}'.")
        {
            this._customerCode = customerCode;
            this._customerName = customerName;
        }
    }
}
