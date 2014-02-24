using NHibernate;

namespace DIMS.Data.DataAccess
{
    /// <summary>
	/// An NHibernate implementation of IGenericTransaction.
	/// </summary>
	public sealed class Transaction
	{
		private readonly ITransaction _transaction;

		public Transaction(ITransaction transaction)
		{
			_transaction = transaction;
		}

		public void Commit()
		{
			_transaction.Commit();
		}

		public void Dispose()
		{
			_transaction.Dispose();
		}

		public void Rollback()
		{
			_transaction.Rollback();
		}
	}
}
