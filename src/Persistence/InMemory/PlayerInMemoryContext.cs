﻿// <copyright file="PlayerInMemoryContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// In-memory context implementation for <see cref="IPlayerContext"/>.
    /// </summary>
    public class PlayerInMemoryContext : InMemoryContext, IPlayerContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerInMemoryContext"/> class.
        /// </summary>
        /// <param name="manager">The manager which holds the memory repositories.</param>
        public PlayerInMemoryContext(InMemoryRepositoryManager manager)
            : base(manager)
        {
        }

        /// <inheritdoc/>
        public LetterBody GetLetterBodyByHeaderId(Guid headerId)
        {
            return this.Manager.GetRepository<LetterBody>().GetAll().FirstOrDefault(body => body.Header.Id == headerId);
        }

        /// <inheritdoc/>
        public Account GetAccountByLoginName(string loginName, string password)
        {
            return this.Manager.GetRepository<Account>().GetAll().FirstOrDefault(account => account.LoginName == loginName && BCrypt.Net.BCrypt.Verify(password, account.PasswordHash));
        }

        /// <inheritdoc/>
        public IEnumerable<Account> GetAccountsOrderedByLoginName(int skip, int count)
        {
            return this.Manager.GetRepository<Account>().GetAll().OrderBy(a => a.LoginName).Skip(skip).Take(count);
        }
    }
}