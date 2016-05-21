using UnityEngine;
using System.Reflection;
using System.Text;
using System.Collections.Generic;

//https://github.com/nobnak/ParameterUnity
namespace DataUI
{
	public class FieldEditor
	{
		public enum FieldKindEnum
		{
			Int,
			Float,
			Bool,
			Vector2,
			Vector3,
			Vector4,
			Matrix,
			Color,
			Enum,
			Unknown

		}

		public const BindingFlags BINDING = BindingFlags.Public | BindingFlags.Instance;

		public readonly System.Object data;
		public readonly List<System.Action> GuiFields = new List<System.Action> ();

		FieldInfo[] _fieldInfos;

		public FieldEditor (System.Object data)
		{
			this.data = data;
			_fieldInfos = data.GetType ().GetFields (BINDING);
			for (var i = 0; i < _fieldInfos.Length; i++)
				GuiFields.Add (GenerateGUI (_fieldInfos [i]));
		}

		public void OnGUI ()
		{
			foreach (var gf in GuiFields)
				gf ();
		}

		public System.Action GenerateGUI (FieldInfo fi)
		{
			var fieldKind = EstimateFieldKind (fi);
			switch (fieldKind) {
			case FieldKindEnum.Int:
				return GenerateGUIInt (fi);
			case FieldKindEnum.Float:
				return GenerateGUIFloat (fi);
			case FieldKindEnum.Vector2:
				return GenerateGUIVector2 (fi);
			case FieldKindEnum.Vector3:
				return GenerateGUIVector3 (fi);
			case FieldKindEnum.Vector4:
				return GenerateGUIVector4 (fi);
			case FieldKindEnum.Matrix:
				return GenerateGUIMatrix4x4 (fi);
			case FieldKindEnum.Color:
				return GenerateGUIColor (fi);
			case FieldKindEnum.Bool:
				return GenerateGUIBool (fi);
			case FieldKindEnum.Enum:
				return GenerateGUIEnum (fi);
			default:
				return GenerateGUIUnsupported (fi);
			}
		}

		public System.Action GenerateGUIInt (FieldInfo fi)
		{
			var textInt = new TextInt ((int)fi.GetValue (data));
			return () => {
				GUILayout.BeginHorizontal ();
				GUILayout.Label (string.Format ("{0} ", fi.Name), GUILayout.ExpandWidth (false));
				textInt.StrValue = GUILayout.TextField (textInt.StrValue, GUILayout.ExpandWidth (true), GUILayout.MinWidth (30f));
				GUILayout.EndHorizontal ();
				fi.SetValue (data, textInt.Value);
			};
		}

		public System.Action GenerateGUIVector2 (FieldInfo fi)
		{
			var textVector = new TextVector ((Vector2)fi.GetValue (data));
			return () => {
				GUILayout.BeginHorizontal ();
				GUILayout.Label (string.Format ("{0} ", fi.Name), GUILayout.ExpandWidth (false));
				for (var i = 0; i < 2; i++)
					textVector [i] = GUILayout.TextField (textVector [i], GUILayout.ExpandWidth (true), GUILayout.MinWidth (30f));
				GUILayout.EndHorizontal ();
				fi.SetValue (data, (Vector2)textVector.Value);
			};
		}

		public System.Action GenerateGUIVector3 (FieldInfo fi)
		{
			var textVector = new TextVector ((Vector3)fi.GetValue (data));
			return () => {
				GUILayout.BeginHorizontal ();
				GUILayout.Label (string.Format ("{0} ", fi.Name), GUILayout.ExpandWidth (false));
				for (var i = 0; i < 3; i++)
					textVector [i] = GUILayout.TextField (textVector [i], GUILayout.ExpandWidth (true), GUILayout.MinWidth (30f));
				GUILayout.EndHorizontal ();
				fi.SetValue (data, (Vector3)textVector.Value);
			};
		}

		public System.Action GenerateGUIVector4 (FieldInfo fi)
		{
			var textVector = new TextVector ((Vector4)fi.GetValue (data));
			return () => {
				GUILayout.BeginHorizontal ();
				GUILayout.Label (string.Format ("{0} ", fi.Name), GUILayout.ExpandWidth (false));
				for (var i = 0; i < 4; i++)
					textVector [i] = GUILayout.TextField (textVector [i], GUILayout.ExpandWidth (true), GUILayout.MinWidth (30f));
				GUILayout.EndHorizontal ();
				fi.SetValue (data, textVector.Value);
			};
		}

		public System.Action GenerateGUIMatrix4x4 (FieldInfo fi)
		{
			var textMatrix = new TextMatrix ((Matrix4x4)fi.GetValue (data));
			return () => {
				GUILayout.BeginHorizontal ();
				GUILayout.Label (string.Format ("{0} ", fi.Name), GUILayout.ExpandWidth (false));
				GUILayout.BeginVertical ();
				for (var y = 0; y < 4; y++) {
					GUILayout.BeginHorizontal ();
					for (var x = 0; x < 4; x++) {
						textMatrix [x + y * 4] = GUILayout.TextField (
							textMatrix [x + y * 4], GUILayout.ExpandWidth (true), GUILayout.MinWidth (30f));
					}
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndVertical ();
				GUILayout.EndHorizontal ();
				fi.SetValue (data, textMatrix.Value);
			};
		}

		public System.Action GenerateGUIColor (FieldInfo fi)
		{
			var textVector = new TextVector ((Color)fi.GetValue (data));
			return () => {
				var c = (Color)textVector.Value;
				GUILayout.BeginVertical ();
				GUILayout.BeginHorizontal ();
				GUILayout.Label (string.Format ("{0} ", fi.Name), GUILayout.ExpandWidth (false));
				var prevColor = GUI.color;
				GUI.color = new Color (c.r, c.g, c.b);
				GUILayout.Label ("■■■■■■", GUILayout.ExpandWidth (false));
				GUI.color = new Color (c.a, c.a, c.a);
				GUILayout.Label ("■■", GUILayout.ExpandWidth (false));
				GUI.color = prevColor;
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				for (var i = 0; i < 4; i++)
					textVector [i] = GUILayout.TextField (textVector [i], GUILayout.ExpandWidth (true), GUILayout.MinWidth (30f));
				GUILayout.EndHorizontal ();
				GUILayout.EndVertical ();
				fi.SetValue (data, (Color)textVector.Value);
			};
		}

		public System.Action GenerateGUIFloat (FieldInfo fi)
		{
			var textFloat = new TextFloat ((float)fi.GetValue (data));
			return () => {
				GUILayout.BeginHorizontal ();
				GUILayout.Label (string.Format ("{0} ", fi.Name), GUILayout.ExpandWidth (false));
				textFloat.StrValue = GUILayout.TextField (textFloat.StrValue, GUILayout.ExpandWidth (true), GUILayout.MinWidth (30f));
				GUILayout.EndHorizontal ();
				fi.SetValue (data, textFloat.Value);
			};
		}

		public System.Action GenerateGUIBool (FieldInfo fi)
		{
			var toggle = (bool)fi.GetValue (data);
			return () => {
				GUILayout.BeginHorizontal ();
				toggle = GUILayout.Toggle (toggle, string.Format ("{0} ", fi.Name));
				GUILayout.EndHorizontal ();
				fi.SetValue (data, toggle);
			};
		}

		public System.Action GenerateGUIEnum (FieldInfo fi)
		{
			var enumType = fi.FieldType;
			var list = new StringBuilder ();
			foreach (var selection in System.Enum.GetValues(enumType))
				list.AppendFormat ("{0}({1}) ", selection, (int)selection);
			var textInt = new TextInt ((int)fi.GetValue (data));
			return () => {
				var enumValue = System.Enum.ToObject (enumType, textInt.Value);
				GUILayout.BeginHorizontal ();
				GUILayout.Label (string.Format ("{0} ", fi.Name), GUILayout.ExpandWidth (false));
				textInt.StrValue = GUILayout.TextField (textInt.StrValue, GUILayout.ExpandWidth (true), GUILayout.MinWidth (30f));
				GUILayout.Label (string.Format ("{0}({1})", enumValue, textInt.Value));
				GUILayout.EndHorizontal ();
				GUILayout.Label (list.ToString ());
				fi.SetValue (data, enumValue);
			};
		}

		public System.Action GenerateGUIUnsupported (FieldInfo fi)
		{
			return () => {
				GUILayout.BeginHorizontal ();
				GUILayout.Label (string.Format ("Unsupported Field : {0} of {1}", fi.Name, fi.FieldType.Name));
				GUILayout.EndHorizontal ();
			};
		}

		public FieldKindEnum EstimateFieldKind (FieldInfo fi)
		{
			var fieldType = fi.FieldType;
			if (fieldType.IsPrimitive) {
				if (fieldType == typeof(int))
					return FieldKindEnum.Int;
				if (fieldType == typeof(float))
					return FieldKindEnum.Float;
				if (fieldType == typeof(bool))
					return FieldKindEnum.Bool;
				return FieldKindEnum.Unknown;
			}
			if (fieldType.IsEnum)
				return FieldKindEnum.Enum;
			if (fieldType.IsValueType) {
				if (fieldType == typeof(Color))
					return FieldKindEnum.Color;
				if (fieldType == typeof(Vector2))
					return FieldKindEnum.Vector2;
				if (fieldType == typeof(Vector3))
					return FieldKindEnum.Vector3;
				if (fieldType == typeof(Vector4))
					return FieldKindEnum.Vector4;
				if (fieldType == typeof(Matrix4x4))
					return FieldKindEnum.Matrix;
			}

			return FieldKindEnum.Unknown;
		}
	}
}
