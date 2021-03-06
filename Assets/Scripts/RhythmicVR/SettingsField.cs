using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RhythmicVR {
	/// <summary>
	/// Defines a settings UI element
	/// </summary>
	[Serializable]
	public class SettingsField {
		public string name;
		public UiType type;
		public MenuPage page;
		public GameObject prefab;
		public GameObject initializedObject;
		public float maxValue = 1;
		public float minValue;
		[NonSerialized] public UnityAction buttonCall;
		[NonSerialized] public UnityAction<float> floatCall;
		[NonSerialized] public UnityAction<string> stringCall;
		[NonSerialized] public UnityAction<int, float> vectorNCall; // int = enumerator, float = value
		[NonSerialized] public UnityAction<int> enumCall;
		[NonSerialized] public UnityAction<bool> boolCall;
		public string menuPath;
		private object initialValue;

		/// <summary>
		/// Initialize the ui field
		/// </summary>
		/// <param name="name">String used as label</param>
		/// <param name="type">The type of element to use (int, string, bool, etc)</param>
		/// <param name="prefab">The prefab to use for generating that element</param>
		/// <param name="menuPath">The path for in which menu to place this element</param>
		/// <param name="initialValue">The initial value (e.g. for int: 30, for string: "C:/myFolder", for Dropdown/Enum: 2(integer index))</param>
		public SettingsField(string name = null, UiType type = default, GameObject prefab = null, string menuPath = null,object initialValue = null) {
			this.name = name;
			this.type = type;
			this.prefab = prefab;
			this.menuPath = menuPath;
			this.initialValue = initialValue;
		}

		public void SetupListeners() {
			InputField input;
			Slider slider;

			SetValues(initialValue);
			
			switch (type) {
				case UiType.Button:
					initializedObject.GetComponentInChildren<Button>().onClick.AddListener(buttonCall);
					break;
				case UiType.Vector3:
					var allFloatInputs = initializedObject.GetComponentsInChildren<InputField>();
					for (var i = 0; i < allFloatInputs.Length; i++) {
						int i2 = i;
						allFloatInputs[i].onValueChanged.AddListener(delegate(string arg0) { vectorNCall(i2, float.Parse(arg0)); });
					}
					break;
				case UiType.Color:
					break;
				case UiType.Text:
					initializedObject.GetComponentInChildren<InputField>().onValueChanged.AddListener(stringCall);
					break;
				case UiType.Int:
					input = initializedObject.GetComponentInChildren<InputField>();
					slider = initializedObject.GetComponentInChildren<Slider>();
					
					input.onValueChanged.AddListener(delegate(string arg0) { 
						floatCall(float.Parse(arg0));
						slider.value = float.Parse(arg0);
					});
					slider.onValueChanged.AddListener(delegate(float arg0) { 
						floatCall(arg0);
						input.text = arg0.ToString();
					});
					break;
				case UiType.Float:
					input = initializedObject.GetComponentInChildren<InputField>();
					slider = initializedObject.GetComponentInChildren<Slider>();
					
					input.onValueChanged.AddListener(delegate(string arg0) { 
						floatCall(float.Parse(arg0));
						slider.value = float.Parse(arg0);
					});
					slider.onValueChanged.AddListener(delegate(float arg0) { 
						floatCall(arg0);
						input.text = arg0.ToString();
					});
					break;
				case UiType.Enum:
					initializedObject.GetComponentInChildren<Dropdown>().onValueChanged.AddListener(enumCall);
					break;
				case UiType.Bool:
					initializedObject.GetComponentInChildren<Toggle>().onValueChanged.AddListener(boolCall);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void SetValues(object value) {
			if (value == null) return;
			switch (type) {
				case UiType.Button:
					break;
				case UiType.Vector3:
					float[] initialValues = new[] {0f};
						
					var initialValuesVec3 = (Vector3) value;
					initialValues = new[] {initialValuesVec3.x, initialValuesVec3.y, initialValuesVec3.z};

					var allFloatInputs = initializedObject.GetComponentsInChildren<InputField>();
					for (var i = 0; i < allFloatInputs.Length; i++) {
						int i2 = i;
						allFloatInputs[i].text = initialValues[i2].ToString();
					}

					break;
				case UiType.Color:
					break;
				case UiType.Text:
					initializedObject.GetComponentInChildren<InputField>().text = (string) value;
					break;
				case UiType.Int:
					initializedObject.GetComponentInChildren<InputField>().text = ((int) value).ToString();
					initializedObject.GetComponentInChildren<Slider>().value = (int) value;
					break;
				case UiType.Float:
					initializedObject.GetComponentInChildren<InputField>().text = ((float) value).ToString();
					initializedObject.GetComponentInChildren<Slider>().value = (float) value;
					break;
				case UiType.Enum:
					initializedObject.GetComponentInChildren<Dropdown>().value = (int) value;
					break;
				case UiType.Bool:
					initializedObject.GetComponentInChildren<Toggle>().isOn = (bool) value;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}