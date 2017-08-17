using UnityEngine;
using System.Collections;

namespace Softdrink{

	public class Render_Controller : MonoBehaviour {

		[TooltipAttribute("Should an image sequence be rendered?")]
		public bool render = false;

		[TooltipAttribute("The playback framerate.")]
		public int playbackFramerate = 30;

		[TooltipAttribute("The data path to the folder to save frames.")]
		public string dataPath = "./Render/";

		[TooltipAttribute("The number of frames to render. \nA value of 0 is used for unlimited.")]
		public int desiredFrames = 100;

		[TooltipAttribute("Exit Play after the render finishes?")]
		public bool exitOnRenderCompletion = true;

		[TooltipAttribute("Show detailed render info (elapsed and estimated time) in the console.")]
		public bool useDetailedInfo = true;

		private float elapsedTime = 0f;
		private float percentageComplete = 0f;
		private float estimatedTime = 0f;
		private float ratio = 0f;

		private bool hasShownRenderCompleteMessage = false;

		void Awake(){
			if(render){
				Time.captureFramerate = playbackFramerate;

				System.IO.Directory.CreateDirectory(dataPath);
				Debug.Log("Created or found render directory...", this);
			}

			if(render) StartCoroutine(renderRoutine());
		}
		
		void Update () {
			if(desiredFrames > 0){
				if(Time.frameCount > desiredFrames){
					render = false;
					if(!hasShownRenderCompleteMessage){
						Debug.Log("Render completed! \n" + elapsedTime.ToString("F2") + "s elapsed.", this);
						if(exitOnRenderCompletion){
							#if UNITY_EDITOR
								UnityEditor.EditorApplication.isPlaying = false;
							#endif
							Application.Quit();
						}
						hasShownRenderCompleteMessage = true;
					}
				}
			}
			
		 	if(render) StartCoroutine(renderRoutine());
		}

		void CalcProgress(){
			percentageComplete = (float)Time.frameCount / (float)desiredFrames;
			elapsedTime = Time.unscaledTime;

			if(desiredFrames != 0) ratio = percentageComplete/elapsedTime;

			if(desiredFrames != 0) estimatedTime = (1.0f - percentageComplete)/ratio;
		}

		IEnumerator renderRoutine(){
			if(!render) yield break;
			yield return new WaitForEndOfFrame();


			 // Append filename to folder name (format is '0005 shot.png"')
	        string name = string.Format("{0}/DMA156_{1:D04}.png", dataPath, Time.frameCount);

	        CalcProgress();

	        if(Time.frameCount % 10 == 1){
	        	string info = "Rendered frame " + Time.frameCount;
	        	if(desiredFrames != 0) info += "of " + desiredFrames;
	        	if(useDetailedInfo){
	        		info += "\nRender is " + (percentageComplete*100f).ToString("F2") + "% complete";
	        		info += "\t" + elapsedTime.ToString("F2") + "s Elapsed; ";
	        		if(desiredFrames != 0) info += estimatedTime.ToString("F2") + "s Estimated remain";
	        	}
			    Debug.Log(info, this);
			}
	        
		    // Capture the screenshot to the specified file.
	        Application.CaptureScreenshot(name);
		}
	}

}
