using System.Threading.Tasks;
using Identity.Application.Interfaces;
using Identity.Core.DomainServices;
using Identity.Core.Repositories;
using Identity.Core.Entities;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Identity.Infrastructure")]

namespace Identity.Application.Services
{
    public class TwoFactorAuthService : ITwoFactorAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly TwoFactorDomainService _twoFactorDomain;

        public TwoFactorAuthService(
            IUserRepository userRepository,
            TwoFactorDomainService twoFactorDomain)
        {
            _userRepository = userRepository;
            _twoFactorDomain = twoFactorDomain;
        }

        public async Task<TwoFASetupResult> EnableTwoFactorAsync(string userId)
        {
            
            Guid userGuid = Guid.Parse(userId); 

            User? user = await _userRepository.GetByIdAsync(userGuid);

            if (user is null)
            {
                    throw new InvalidOperationException($"User with ID {userId} not found."); 
            }       
            var (secret, qrCode) = _twoFactorDomain.GenerateSetupFor(user.Username);



            return new TwoFASetupResult(
                ManualKey: secret,
                QrCodeImageBase64: qrCode
            );
        }

        public Task<TwoFAVerificationResult> VerifySetupAsync(string userId, string code)
        {
            // U pravoj implementaciji bi se ovdje pročitao secret iz baze
            // za datog userId, npr. var secret = user.TwoFactorSecret;
            // Za sada koristimo "dummy" vrijednost – ITotpProvider je demo
            // i gleda samo u code ("123456").

            const string demoSecret = "IGNORED_IN_DEMO";

            var ok = _twoFactorDomain.VerifyCode(demoSecret, code);

            if (!ok)
            {
                return Task.FromResult(
                    new TwoFAVerificationResult(
                        Success: false,
                        Message: "Invalid or expired code. Please try again."
                    )
                );
            }

            // U pravoj implementaciji bi se ovdje setao user.TwoFactorEnabled = true itd.
            return Task.FromResult(new TwoFAVerificationResult(true, null));
        }

        public Task<TwoFAVerificationResult> VerifyLoginAsync(string userId, string code)
        {
            // Isto kao gore – u realnoj varijanti bi se secret čitao iz baze.
            const string demoSecret = "IGNORED_IN_DEMO";

            var ok = _twoFactorDomain.VerifyCode(demoSecret, code);

            if (!ok)
            {
                return Task.FromResult(
                    new TwoFAVerificationResult(
                        Success: false,
                        Message: "Invalid or expired code. Please try again."
                    )
                );
            }

            // Ovdje se eventualno može logirati uspješan 2FA login.
            return Task.FromResult(new TwoFAVerificationResult(true, null));
        }
    }
}
