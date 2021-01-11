using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Tgrc.UnityEditorUtils.Assets
{
	class ManualSaveAssets
	{
		[MenuItem("AssetDatabase/Save all assets &s")]
		private static void SaveAssets()
		{
			AssetDatabase.SaveAssets();
			Debug.Log("Saved all assets.");
		}
	}
}
