using MABMoney.Domain;
using MABMoney.Domain.Abstract;
using System;
using System.Collections.Generic;

namespace MABMoney.Data.Concrete
{
    public class BudgetRepository : BaseRepository, IBudgetRepository
    {
        public BudgetRepository(string connectionString, int userId) : base(connectionString, userId) { }

        public IEnumerable<Budget> All()
        {
            return GetEnumerable<Budget>("mm_Budgets_Read", new { UserID = _userId });
        }

        public Budget Get(int id)
        {
            return GetSingle<Budget>("mm_Budgets_Read", new { UserID = _userId, BudgetID = id });
        }

        public Budget Add(Budget budget)
        {
            return AddOrUpdate<Budget>("mm_Budgets_Create", new {
                UserID = _userId,
                GUID = budget.GUID,
                Account_AccountID = budget.Account_AccountID,
                Start = budget.Start,
                End = budget.End
            });
        }

        public Budget Update(Budget budget)
        {
            return AddOrUpdate<Budget>("mm_Budgets_Update", new {
                UserID = _userId,
                BudgetID = budget.BudgetID,
                Account_AccountID = budget.Account_AccountID,
                Start = budget.Start,
                End = budget.End
            });
        }

        public Budget Delete(int id)
        {
            return AddOrUpdate<Budget>("mm_Budgets_Delete", new { UserID = _userId, BudgetID = id });
        }

        public Budget GetLatest(int accountId, DateTime date)
        {
            return GetSingle<Budget>("mm_Budgets_Read_Latest", new { 
                UserID = _userId, 
                AccountID = accountId,
                Date = date
            });
        }

        public int GetBudgetCount(int accountId)
        {
            return GetScalar<int>("mm_Budgets_Count", new { UserID = _userId, AccountID = accountId });
        }
    }
}
