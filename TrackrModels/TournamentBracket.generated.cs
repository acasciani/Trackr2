#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the ClassGenerator.ttinclude code generation file.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Common;
using System.Collections.Generic;
using Telerik.OpenAccess;
using Telerik.OpenAccess.Metadata;
using Telerik.OpenAccess.Data.Common;
using Telerik.OpenAccess.Metadata.Fluent;
using Telerik.OpenAccess.Metadata.Fluent.Advanced;
using TrackrModels;

namespace TrackrModels	
{
	[System.Serializable()]
	public partial class TournamentBracket : System.Runtime.Serialization.ISerializable
	{
		private int _tournamentBracketID;
		public virtual int TournamentBracketID
		{
			get
			{
				return this._tournamentBracketID;
			}
			set
			{
				this._tournamentBracketID = value;
			}
		}
		
		private string _bracketName;
		public virtual string BracketName
		{
			get
			{
				return this._bracketName;
			}
			set
			{
				this._bracketName = value;
			}
		}
		
		private short _teamLimit;
		public virtual short TeamLimit
		{
			get
			{
				return this._teamLimit;
			}
			set
			{
				this._teamLimit = value;
			}
		}
		
		private int _tournamentID;
		public virtual int TournamentID
		{
			get
			{
				return this._tournamentID;
			}
			set
			{
				this._tournamentID = value;
			}
		}
		
		private Tournament _tournament;
		public virtual Tournament Tournament
		{
			get
			{
				return this._tournament;
			}
			set
			{
				this._tournament = value;
			}
		}
		
		private IList<TournamentTeam> _tournamentTeams = new List<TournamentTeam>();
		public virtual IList<TournamentTeam> TournamentTeams
		{
			get
			{
				return this._tournamentTeams;
			}
		}
		
		#region ISerializable Implementation
		
		public TournamentBracket()
		{
		}
		
		protected TournamentBracket(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			this.TournamentBracketID = info.GetInt32("TournamentBracketID");
			this.BracketName = info.GetString("BracketName");
			this.TeamLimit = info.GetInt16("TeamLimit");
			this.TournamentID = info.GetInt32("TournamentID");
			CustomizeDeserializationProcess(info, context);
		}
		
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("TournamentBracketID", this.TournamentBracketID, typeof(int));
			info.AddValue("BracketName", this.BracketName, typeof(string));
			info.AddValue("TeamLimit", this.TeamLimit, typeof(short));
			info.AddValue("TournamentID", this.TournamentID, typeof(int));
			CustomizeSerializationProcess(info, context);
		}
		
		partial void CustomizeSerializationProcess(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
		partial void CustomizeDeserializationProcess(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
		#endregion
	}
}
#pragma warning restore 1591
