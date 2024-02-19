﻿namespace HomeBankingMindHub.Models
{
    public class DBInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client 
                    { 
                        Email = "vcoronado@gmail.com", 
                        FirstName="Victor", 
                        LastName="Coronado", 
                        Password="123456"
                    },

                    new Client
                    {
                        Email = "rmarcos@gmail.com",
                        FirstName="Marcos",
                        LastName="Rotts",
                        Password="13579"
                    },

                    new Client
                    {
                        Email = "gliliana@gmail.com",
                        FirstName="Liliana",
                        LastName="Gutenberg",
                        Password="246810"
                    }

                };

                context.Clients.AddRange(clients);
                //guardamos
                context.SaveChanges();
            }

            if (!context.Accounts.Any())
            {
                var accountVictor = context.Clients.FirstOrDefault(client => client.Email == "vcoronado@gmail.com");
                var accountMarcos = context.Clients.FirstOrDefault(client => client.Email == "rmarcos@gmail.com");
                var accountLiliana = context.Clients.FirstOrDefault(client => client.Email == "gliliana@gmail.com");

                if (accountVictor != null && accountMarcos != null && accountLiliana != null)
                {
                    var accounts = new Account[]
                    {
                        new Account
                        {
                            ClientId = accountVictor.Id,
                            CreationDate = DateTime.Now,
                            Number = "VIN001",
                            Balance = 0
                        },
                        new Account
                        {
                            ClientId = accountMarcos.Id,
                            CreationDate = new DateTime(2022, 5, 10, 14, 9, 55),
                            Number = "VIN002",
                            Balance = 50000
                        },
                        new Account
                        {
                            ClientId = accountLiliana.Id,
                            CreationDate = new DateTime(2023, 7, 16, 11, 10, 32),
                            Number = "VIN003",
                            Balance = 25000
                        }
                    };

                    foreach (Account account in accounts)
                    {
                        context.Accounts.Add(account);
                    }

                    context.SaveChanges();
                }
            }

            if (!context.Transactions.Any())
            {
                var account1 = context.Accounts.FirstOrDefault(account => account.Number == "VIN001");

                if (account1 != null)
                {
                    var transactions = new Transaction[]
                    {
                        new Transaction { AccountId= account1.Id, Amount = 10000, Date= DateTime.Now.AddHours(-5), Description = "Transferencia reccibida", Type = TransactionType.CREDIT },
                        new Transaction { AccountId= account1.Id, Amount = -2000, Date= DateTime.Now.AddHours(-6), Description = "Compra en tienda mercado libre", Type = TransactionType.DEBIT },
                        new Transaction { AccountId= account1.Id, Amount = -3000, Date= DateTime.Now.AddHours(-7), Description = "Compra en tienda xxxx", Type = TransactionType.DEBIT },
                    };

                    foreach (Transaction transaction in transactions)
                    {
                        context.Transactions.Add(transaction);
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}
