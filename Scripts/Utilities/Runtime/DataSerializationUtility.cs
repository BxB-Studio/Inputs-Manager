#region Namespaces

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

#endregion

namespace Utilities
{
	public class DataSerializationUtility<T> where T : class
	{
		#region Variables

		private readonly string path;
		private readonly string name;

		#endregion

		#region Methods

		#region Utilities

		public bool SaveOrCreate(T data)
		{
			CheckValidity();

			string fullPath = $"{path}/{name}";
			FileStream stream = null;

			try
			{
				stream = File.Open(fullPath, FileMode.OpenOrCreate);

				BinaryFormatter formatter = new BinaryFormatter();

				formatter.Serialize(stream, data);

				return true;
			}
			catch (Exception e)
			{
				throw e;
			}
			finally
			{
				if (stream != null)
					stream.Close();
			}
		}
		public T Load()
		{
			CheckValidity();

			string fullPath = $"{path}/{name}";

			if (!File.Exists(fullPath))
				throw new ArgumentException("The given file name argument doesn't exist");

			FileStream stream = null;

			try
			{

				stream = File.Open(fullPath, FileMode.OpenOrCreate);
				BinaryFormatter formatter = new BinaryFormatter();
				T data = formatter.Deserialize(stream) as T;

				return data;
			}
			catch (Exception e)
			{
				throw e;
			}
			finally
			{
				if (stream != null)
					stream.Close();
			}
		}

		private void CheckValidity()
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			FileAttributes fileAttributes = File.GetAttributes(path);

			if (!fileAttributes.HasFlag(FileAttributes.Directory))
				throw new ArgumentException($"The `path` argument of value \"{path}\" is not a valid directory");

			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name", "The `name` argument cannot be null");
		}

        #endregion

        #region Constructors & Operators

        #region Constructors

        public DataSerializationUtility(string path, string name)
		{
			this.path = path;
			this.name = name;

			CheckValidity();
		}

        #endregion

        #region Operators

		public static implicit operator bool(DataSerializationUtility<T> serializationUtility) => serializationUtility != null;

        #endregion

        #endregion

        #endregion
    }
}
