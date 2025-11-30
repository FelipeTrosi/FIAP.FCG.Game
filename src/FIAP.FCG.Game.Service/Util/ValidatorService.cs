using System.ComponentModel.DataAnnotations;

namespace FIAP.FCG.Game.Service.Util
{
    public static class ValidatorService
    {
        public static bool IsValidEmail(string email) 
            => new EmailAddressAttribute().IsValid(email);

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            bool hasLetter = false, hasDigit = false, hasSpecial = false;

            foreach (char c in password)
            {
                if (char.IsLetter(c)) hasLetter = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecial = true;

                if (hasLetter && hasDigit && hasSpecial)
                    return true;
            }

            return false;
        }

    }
}
