<Query Kind="Program">
  <Reference Relative="MABMoney.Domain\bin\Debug\MABMoney.Domain.dll">C:\Src\MABMoney\MABMoney.Domain\bin\Debug\MABMoney.Domain.dll</Reference>
  <Namespace>MABMoney.Domain</Namespace>
  <Namespace>MABMoney.Domain.Abstract</Namespace>
  <Namespace>MABMoney.Domain.Extensions</Namespace>
</Query>

void Main()
{
	var factory = new Factory();
	
	var account = factory.CreateAccount(1, "TEST", AccountType.Current, 100.00M);
	
	
	account.Dump();

	var transaction1 = new MABMoney.Domain.Transaction {
		Date = DateTime.Now,
		Amount = -10M,
		Description = "Test Transaction 1"
	};
	
	account.AddTransaction(transaction1);
	
	account.Dump();
	
	var transaction2 = new MABMoney.Domain.Transaction {
		Date = DateTime.Now.AddDays(2),
		Amount = 60M,
		Description = "Test Transaction 2"
	};

	account.AddTransaction(transaction2);

	account.Dump();
}

