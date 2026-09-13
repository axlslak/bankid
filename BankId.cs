using System;
using System.Globalization;
using System.Linq;
using AOSharp.Core;
using AOSharp.Core.UI;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.ChatMessages;

namespace BankId
{
    public sealed class Main : AOPluginEntry
    {
        private const string Recipient = "Apcmanager";
        private int? _pendingInstance;

        public override void Run()
        {
            Chat.RegisterCommand("bankid", OnCommand);
            Network.ChatMessageReceived += OnChatMessage;
        }

        private void OnCommand(string command, string[] arguments, ChatWindow window)
        {
            try
            {
                // Read the full client's live list only when the user asks.
                var dynels = DynelManager.AllDynels.ToList();
                if (!dynels.Any(d => string.Equals(d.Name, "Kbcentral", StringComparison.OrdinalIgnoreCase)))
                {
                    Chat.WriteLine("BankId: Kbcentral is not visible.");
                    return;
                }

                var bank = dynels.FirstOrDefault(d => string.Equals(d.Name,
                    "Rubi-Ka Banking Service Terminal", StringComparison.OrdinalIgnoreCase));
                if (bank == null)
                {
                    Chat.WriteLine("BankId: Rubi-Ka Banking Service Terminal is not visible.");
                    return;
                }

                _pendingInstance = bank.Identity.Instance;
                // Resolve the recipient by name; no hard-coded character ID.
                Network.Send(new LookupMessage { Name = Recipient });
            }
            catch (Exception ex)
            {
                _pendingInstance = null;
                Chat.WriteLine("BankId: " + ex.Message);
            }
        }

        private void OnChatMessage(object sender, ChatMessageBody message)
        {
            var lookup = message as LookupMessage;
            if (!_pendingInstance.HasValue || lookup == null ||
                !string.Equals(lookup.Name, Recipient, StringComparison.OrdinalIgnoreCase))
                return;

            int instance = _pendingInstance.Value;
            _pendingInstance = null; // Consume once, before sending the tell.
            if (lookup.Id == 0 || lookup.Id == uint.MaxValue)
            {
                Chat.WriteLine("BankId: could not resolve Apcmanager.");
                return;
            }

            try
            {
                Chat.SendPrivateMessage(lookup.Id,
                    "bankid " + instance.ToString(CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                Chat.WriteLine("BankId: tell failed: " + ex.Message);
            }
        }

        public override void Teardown()
        {
            _pendingInstance = null;
            Network.ChatMessageReceived -= OnChatMessage;
        }
    }
}
