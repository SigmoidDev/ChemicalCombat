using System.Collections.Generic;
using Sigmoid.Utilities;
using UnityEngine;

namespace Sigmoid.Buffs
{
    /// <summary>
    /// Holds information about the tick rates and damages of all damage over times
    /// </summary>
	public class DotManager : Singleton<DotManager>
	{
		[SerializeField] private List<ScriptableDot> dotList;
		private Dictionary<DotType, ScriptableDot> dotInfo;

        private void Awake()
		{
			dotInfo = new Dictionary<DotType, ScriptableDot>();
			foreach(ScriptableDot dot in dotList)
				dotInfo.Add(dot.relatedDot, dot);
		}

		public static ScriptableDot Get(DotType dot) => Instance.dotInfo.TryGetValue(dot, out ScriptableDot info) ? info : null;
	}

    public enum DotType
	{
		Dissolving,
		Burning,
		Corroding,
		Poisoned
	}
}
