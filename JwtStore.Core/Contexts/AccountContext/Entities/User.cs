using JwtStore.Core.AccountContext.ValueObjects;
using JwtStore.Core.Contexts.AccountContext.ValueObjects;
using JwtStore.Core.Contexts.SharedContext.Entities;

namespace JwtStore.Core.Contexts.AccountContext.Entities
{
    public class User : Entity
    {
        protected User() { }
        public User(string email, string? password = null)
        {
            Email = email;
            Password = new Password(password);
        }

        public string Nome { get; private set; } = string.Empty;
        public Email Email { get; private set; } = null!;
        public Password Password { get; private set; } = null!;
        public string Image { get; private set; } = string.Empty;

        public void UpdatePassword(string plainTExtPassword, string code)
        {
            if (!string.Equals(code.Trim(), Password.ResetCode.Trim(), StringComparison.CurrentCultureIgnoreCase))
                throw new Exception("Código de restauração inválido");

            var password = new Password(plainTExtPassword);
            Password = password;
        }

        public void UpdateEmail(Email email)
        {
            Email = email;
        }
    }
}
