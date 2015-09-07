using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using dotless.Core.Utils;
using Microsoft.AspNet.SignalR;
using Uno;
using Uno.Model;
using WebClient.Controllers;

namespace WebClient.SignalR
{
    public class GameHub : Hub
    {
        public void Enter(int id)
        {
            Lobby.Instance.Rooms[id].Players.Single(p => p.Name == Context.RequestCookies["name"].Value).ConnectionId =
                Context.ConnectionId;
            Groups.Add(Context.ConnectionId, id.ToString());
            
        }

        public void Move(int index)
        {
            var lobby = Lobby.Instance;
            var roomIndex = lobby.Rooms.FindIndex(r => r.Players.Any(p => p.Name == Context.RequestCookies["name"].Value));
            var gameSession = lobby.Rooms[roomIndex].GameSession;
            gameSession.Discard(index);
            var topCard = gameSession.Game.DiscardPileTop;
            Clients.OthersInGroup(
                roomIndex
                    .ToString())
                .move(SerializeCard(topCard));
        }

        private string SerializeCard(Card card)
        {
            return $"{(card.Type == CardType.Number ? card.Number.ToString() : card.Type.ToString())} {card.Color}";
        }

        public void Draw()
        {

            var lobby = Lobby.Instance;
            var roomIndex = lobby.Rooms.FindIndex(r => r.Players.Any(p => p.Name == Context.RequestCookies["name"].Value));
            var gameSession = lobby.Rooms[roomIndex].GameSession;
            var currentPlayer = gameSession.CurrentPlayer;
            gameSession.Draw();
            Clients.Caller.draw(SerializeCard(currentPlayer.Cards.Last()));
        }


        public void Pass()
        {
            var lobby = Lobby.Instance;
            var roomIndex = lobby.Rooms.FindIndex(r => r.Players.Any(p => p.Name == Context.RequestCookies["name"].Value));
            var gameSession = lobby.Rooms[roomIndex].GameSession;
            gameSession.Pass();
            Clients.OthersInGroup(roomIndex.ToString()).move();
        }
    }

    internal class Huy : MembershipProvider
    {
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer,
            bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion,
            string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordRetrieval { get; }
        public override bool EnablePasswordReset { get; }
        public override bool RequiresQuestionAndAnswer { get; }
        public override string ApplicationName { get; set; }
        public override int MaxInvalidPasswordAttempts { get; }
        public override int PasswordAttemptWindow { get; }
        public override bool RequiresUniqueEmail { get; }
        public override MembershipPasswordFormat PasswordFormat { get; }
        public override int MinRequiredPasswordLength { get; }
        public override int MinRequiredNonAlphanumericCharacters { get; }
        public override string PasswordStrengthRegularExpression { get; }
    }
}