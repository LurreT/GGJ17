using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[AddComponentMenu("Event/Alpha Raycaster"), ExecuteInEditMode]
public class AlphaRaycaster : GraphicRaycaster
{
	[Header("Alpha test properties")]
	[Range(0, 1), Tooltip("Below that value of alpha components won't react to raycast.")]
	public float AlphaThreshold = .9f;
	[Tooltip("Include material tint color when checking alpha.")]
	public bool IncludeMaterialAlpha;
	[Tooltip("Will test alpha only on objects with Alpha Check component.")]
	public bool SelectiveMode;
	[Tooltip("Show warnings in the console when raycasting objects with a not-readable texture.")]
	public bool ShowTextureWarnings;

	private List<RaycastResult> toExclude = new List<RaycastResult>();

	protected override void OnEnable ()
	{
		base.OnEnable();

		var badGuy = GetComponent<GraphicRaycaster>();
		if (badGuy && badGuy != this) DestroyImmediate(badGuy);
	}

	public override void Raycast (PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		base.Raycast(eventData, resultAppendList);

		toExclude.Clear();

		foreach (var result in resultAppendList)
		{
			var objImage = result.gameObject.GetComponent<Image>();
			if (!objImage) continue;
			if (!objImage.sprite) continue;

			var objAlphaCheck = result.gameObject.GetComponent<AlphaCheck>();
			if (SelectiveMode && !objAlphaCheck) continue;

			try
			{
				var objTrs = result.gameObject.transform as RectTransform;

				// Evaluating pointer position relative to object local space
				Vector3 pointerGPos;
				if (eventCamera)
				{
					var objPlane = new Plane(objTrs.forward, objTrs.position);
					float distance;
					var cameraRay = eventCamera.ScreenPointToRay(eventData.position);
					objPlane.Raycast(cameraRay, out distance);
					pointerGPos = cameraRay.GetPoint(distance);
				}
				else
				{
					pointerGPos = eventData.position;
					float rotationCorrection = (-objTrs.forward.x * (pointerGPos.x - objTrs.position.x) - objTrs.forward.y * (pointerGPos.y - objTrs.position.y)) / objTrs.forward.z;
					pointerGPos += new Vector3(0, 0, objTrs.position.z + rotationCorrection);
				}
				Vector3 pointerLPos = objTrs.InverseTransformPoint(pointerGPos);

				var objTex = objImage.mainTexture as Texture2D;
				Rect texRect;
				// Case for sprites with redundant transparent areas (Unity trims them internally, so we have to handle that)
				if (objImage.sprite.textureRectOffset.sqrMagnitude > 0)
				{
					texRect = objImage.sprite.packed ? new Rect(objImage.sprite.textureRect.xMin - objImage.sprite.textureRectOffset.x,
					objImage.sprite.textureRect.yMin - objImage.sprite.textureRectOffset.y,
					objImage.sprite.textureRect.width + objImage.sprite.textureRectOffset.x * 2f,
					objImage.sprite.textureRect.height + objImage.sprite.textureRectOffset.y * 2f) : objImage.sprite.rect;
				}
				else texRect = objImage.sprite.textureRect;
				var objSize = objTrs.rect.size;

				// Correcting objSize in case "preserve aspect" is enabled 
				if (objImage.preserveAspect)
				{
					if (objSize.x < objSize.y) objSize.y = objSize.x * (texRect.height / texRect.width);
					else objSize.x = objSize.y * (texRect.width / texRect.height);

					// Also we need to cut off empty object space
					var halfPivot = new Vector2(Mathf.Abs(objTrs.pivot.x) == .5f ? 2 : 1, Mathf.Abs(objTrs.pivot.y) == .5f ? 2 : 1);
					if (Mathf.Abs(pointerLPos.x * halfPivot.x) > objSize.x || Mathf.Abs(pointerLPos.y * halfPivot.y) > objSize.y) { toExclude.Add(result); continue; }
				}

				// Evaluating texture coordinates of the targeted spot
				float texCorX = pointerLPos.x + objSize.x * objTrs.pivot.x;
				float texCorY = pointerLPos.y + objSize.y * objTrs.pivot.y;

				#region	TILED_SLICED
				// Will be used if image has a border
				var borderTotalWidth = objImage.sprite.border.x + objImage.sprite.border.z;
				var borderTotalHeight = objImage.sprite.border.y + objImage.sprite.border.w;
				var fillRect = new Rect(objImage.sprite.border.x, objImage.sprite.border.y, 
					Mathf.Clamp(objSize.x - borderTotalWidth, 0f, Mathf.Infinity), 
					Mathf.Clamp(objSize.y - borderTotalHeight, 0f, Mathf.Infinity));
				var isInsideFillRect = objImage.hasBorder && fillRect.Contains(new Vector2(texCorX, texCorY));

				// Correcting texture coordinates in case image is tiled
				if (objImage.type == Image.Type.Tiled)
				{
					if (isInsideFillRect)
					{
						if (!objImage.fillCenter) { toExclude.Add(result); continue; }

						texCorX = objImage.sprite.border.x + (texCorX - objImage.sprite.border.x) % (texRect.width - borderTotalWidth);
						texCorY = objImage.sprite.border.y + (texCorY - objImage.sprite.border.y) % (texRect.height - borderTotalHeight);
					}
					else if (objImage.hasBorder)
					{
						// If objSize is below border size the border areas will shrink
						texCorX *= Mathf.Clamp(borderTotalWidth / objSize.x, 1f, Mathf.Infinity);
						texCorY *= Mathf.Clamp(borderTotalHeight / objSize.y, 1f, Mathf.Infinity);

						if (texCorX > texRect.width - objImage.sprite.border.z  && texCorX < objImage.sprite.border.x + fillRect.width)
							texCorX = objImage.sprite.border.x + (texCorX - objImage.sprite.border.x) % (texRect.width - borderTotalWidth);
						else if (texCorX > objImage.sprite.border.x + fillRect.width)
							texCorX = texCorX - fillRect.width + texRect.width - borderTotalWidth;

						if (texCorY > texRect.height - objImage.sprite.border.w && texCorY < objImage.sprite.border.y + fillRect.height)
							texCorY = objImage.sprite.border.y + (texCorY - objImage.sprite.border.y) % (texRect.height - borderTotalHeight);
						else if (texCorY > objImage.sprite.border.y + fillRect.height)
							texCorY = texCorY - fillRect.height + texRect.height - borderTotalHeight;
					}
					else
					{
						if (texCorX > texRect.width) texCorX %= texRect.width;
						if (texCorY > texRect.height) texCorY %= texRect.height;
					}
				}
				// Correcting texture coordinates in case image is sliced
				else if (objImage.type == Image.Type.Sliced)
				{
					if (isInsideFillRect)
					{
						if (!objImage.fillCenter) { toExclude.Add(result); continue; }

						texCorX = objImage.sprite.border.x + (texCorX - objImage.sprite.border.x) * ((texRect.width - borderTotalWidth) / fillRect.width);
						texCorY = objImage.sprite.border.y + (texCorY - objImage.sprite.border.y) * ((texRect.height - borderTotalHeight) / fillRect.height);
					}
					else
					{
						// If objSize is below border size the border areas will shrink
						texCorX *= Mathf.Clamp(borderTotalWidth / objSize.x, 1f, Mathf.Infinity);
						texCorY *= Mathf.Clamp(borderTotalHeight / objSize.y, 1f, Mathf.Infinity);

						if (texCorX > objImage.sprite.border.x && texCorX < objImage.sprite.border.x + fillRect.width)
							texCorX = objImage.sprite.border.x + (texCorX - objImage.sprite.border.x) * ((texRect.width - borderTotalWidth) / fillRect.width);
						else if (texCorX > objImage.sprite.border.x + fillRect.width)
							texCorX = texCorX - fillRect.width + texRect.width - borderTotalWidth;

						if (texCorY > objImage.sprite.border.y && texCorY < objImage.sprite.border.y + fillRect.height)
							texCorY = objImage.sprite.border.y + (texCorY - objImage.sprite.border.y) * ((texRect.height - borderTotalHeight) / fillRect.height);
						else if (texCorY > objImage.sprite.border.y + fillRect.height)
							texCorY = texCorY - fillRect.height + texRect.height - borderTotalHeight;
					}
				}
				#endregion
				// Correcting texture coordinates by scale in case simple or filled image
				else
				{
					texCorX *= texRect.width / objSize.x;
					texCorY *= texRect.height / objSize.y;
				}

				// For filled images, check if targeted spot is outside of the filled area
				#region FILLED
				if (objImage.type == Image.Type.Filled)
				{
					var nCorX = texRect.height > texRect.width ? texCorX * (texRect.height / texRect.width) : texCorX;
					var nCorY = texRect.width > texRect.height ? texCorY * (texRect.width / texRect.height) : texCorY;
					var nWidth = texRect.height > texRect.width ? texRect.height : texRect.width;
					var nHeight = texRect.width > texRect.height ? texRect.width : texRect.height;

					if (objImage.fillMethod == Image.FillMethod.Horizontal)
					{
						if (objImage.fillOrigin == (int)Image.OriginHorizontal.Left && texCorX / texRect.width > objImage.fillAmount) { toExclude.Add(result); continue; }
						if (objImage.fillOrigin == (int)Image.OriginHorizontal.Right && texCorX / texRect.width < (1 - objImage.fillAmount)) { toExclude.Add(result); continue; }
					}

					if (objImage.fillMethod == Image.FillMethod.Vertical)
					{
						if (objImage.fillOrigin == (int)Image.OriginVertical.Bottom && texCorY / texRect.height > objImage.fillAmount) { toExclude.Add(result); continue; }
						if (objImage.fillOrigin == (int)Image.OriginVertical.Top && texCorY / texRect.height < (1 - objImage.fillAmount)) { toExclude.Add(result); continue; }
					}

					#region RADIAL_90
					if (objImage.fillMethod == Image.FillMethod.Radial90)
					{
						if (objImage.fillOrigin == (int)Image.Origin90.BottomLeft)
						{
							if (objImage.fillClockwise && Mathf.Atan(nCorY / nCorX) / (Mathf.PI / 2) < (1 - objImage.fillAmount)) { toExclude.Add(result); continue; }
							if (!objImage.fillClockwise && Mathf.Atan(nCorY / nCorX) / (Mathf.PI / 2) > objImage.fillAmount) { toExclude.Add(result); continue; }
						}

						if (objImage.fillOrigin == (int)Image.Origin90.TopLeft)
						{
							if (objImage.fillClockwise && nCorY < -(1 / Mathf.Tan((1 - objImage.fillAmount) * Mathf.PI / 2)) * nCorX + nHeight) { toExclude.Add(result); continue; }
							if (!objImage.fillClockwise && nCorY > -(1 / Mathf.Tan(objImage.fillAmount * Mathf.PI / 2)) * nCorX + nHeight) { toExclude.Add(result); continue; }
						}

						if (objImage.fillOrigin == (int)Image.Origin90.TopRight)
						{
							if (objImage.fillClockwise && nCorY > Mathf.Tan((1 - objImage.fillAmount) * Mathf.PI / 2) * (nCorX - nWidth) + nHeight) { toExclude.Add(result); continue; }
							if (!objImage.fillClockwise && nCorY < Mathf.Tan(objImage.fillAmount * Mathf.PI / 2) * (nCorX - nWidth) + nHeight) { toExclude.Add(result); continue; }
						}

						if (objImage.fillOrigin == (int)Image.Origin90.BottomRight)
						{
							if (objImage.fillClockwise && nCorY > (1 / Mathf.Tan((1 - objImage.fillAmount) * Mathf.PI / 2)) * (nWidth - nCorX)) { toExclude.Add(result); continue; }
							if (!objImage.fillClockwise && nCorY < (1 / Mathf.Tan(objImage.fillAmount * Mathf.PI / 2)) * (nWidth - nCorX)) { toExclude.Add(result); continue; }
						}
					}
					#endregion

					#region RADIAL_180
					if (objImage.fillMethod == Image.FillMethod.Radial180)
					{
						if (objImage.fillOrigin == (int)Image.Origin180.Bottom)
						{
							if (objImage.fillClockwise && Mathf.Atan2(nCorY, 2 * (nCorX - nWidth / 2)) < (1 - objImage.fillAmount) * Mathf.PI) { toExclude.Add(result); continue; }
							if (!objImage.fillClockwise && Mathf.Atan2(texCorY, 2 * (nCorX - nWidth / 2)) > objImage.fillAmount * Mathf.PI) { toExclude.Add(result); continue; }
						}

						if (objImage.fillOrigin == (int)Image.Origin180.Left)
						{
							if (objImage.fillClockwise && Mathf.Atan2(nCorX, -2 * (nCorY - nHeight / 2)) < (1 - objImage.fillAmount) * Mathf.PI) { toExclude.Add(result); continue; }
							if (!objImage.fillClockwise && Mathf.Atan2(nCorX, -2 * (nCorY - nHeight / 2)) > objImage.fillAmount * Mathf.PI) { toExclude.Add(result); continue; }
						}

						if (objImage.fillOrigin == (int)Image.Origin180.Top)
						{
							if (objImage.fillClockwise && Mathf.Atan2(nHeight - nCorY, -2 * (nCorX - nWidth / 2)) < (1 - objImage.fillAmount) * Mathf.PI) { toExclude.Add(result); continue; }
							if (!objImage.fillClockwise && Mathf.Atan2(nHeight - nCorY, -2 * (nCorX - nWidth / 2)) > objImage.fillAmount * Mathf.PI) { toExclude.Add(result); continue; }
						}

						if (objImage.fillOrigin == (int)Image.Origin180.Right)
						{
							if (objImage.fillClockwise && Mathf.Atan2(nWidth - nCorX, 2 * (nCorY - nHeight / 2)) < (1 - objImage.fillAmount) * Mathf.PI) { toExclude.Add(result); continue; }
							if (!objImage.fillClockwise && Mathf.Atan2(nWidth - nCorX, 2 * (nCorY - nHeight / 2)) > objImage.fillAmount * Mathf.PI) { toExclude.Add(result); continue; }
						}
					}
					#endregion

					#region RADIAL_360
					if (objImage.fillMethod == Image.FillMethod.Radial360)
					{
						if (objImage.fillOrigin == (int)Image.Origin360.Bottom)
						{
							if (objImage.fillClockwise)
							{
								var angle = Mathf.Atan2(nCorY - nHeight / 2, nCorX - nWidth / 2) + Mathf.PI / 2;
								var checkAngle = Mathf.PI * 2 * (1 - objImage.fillAmount);
								angle = angle < 0 ? Mathf.PI * 2 + angle : angle;
								if (angle < checkAngle) { toExclude.Add(result); continue; }
							}
							if (!objImage.fillClockwise)
							{
								var angle = Mathf.Atan2(nCorY - nHeight / 2, nCorX - nWidth / 2) + Mathf.PI / 2;
								var checkAngle = Mathf.PI * 2 * objImage.fillAmount;
								angle = angle < 0 ? Mathf.PI * 2 + angle : angle;
								if (angle > checkAngle) { toExclude.Add(result); continue; }
							}
						}

						if (objImage.fillOrigin == (int)Image.Origin360.Right)
						{
							if (objImage.fillClockwise)
							{
								var angle = Mathf.Atan2(nCorY - nHeight / 2, nCorX - nWidth / 2);
								var checkAngle = Mathf.PI * 2 * (1 - objImage.fillAmount);
								angle = angle < 0 ? Mathf.PI * 2 + angle : angle;
								if (angle < checkAngle) { toExclude.Add(result); continue; }
							}
							if (!objImage.fillClockwise)
							{
								var angle = Mathf.Atan2(nCorY - nHeight / 2, nCorX - nWidth / 2);
								var checkAngle = Mathf.PI * 2 * objImage.fillAmount;
								angle = angle < 0 ? Mathf.PI * 2 + angle : angle;
								if (angle > checkAngle) { toExclude.Add(result); continue; }
							}
						}

						if (objImage.fillOrigin == (int)Image.Origin360.Top)
						{
							if (objImage.fillClockwise)
							{
								var angle = Mathf.Atan2(nCorY - nHeight / 2, nCorX - nWidth / 2) - Mathf.PI / 2;
								var checkAngle = Mathf.PI * 2 * (1 - objImage.fillAmount);
								angle = angle < 0 ? Mathf.PI * 2 + angle : angle;
								if (angle < checkAngle) { toExclude.Add(result); continue; }
							}
							if (!objImage.fillClockwise)
							{
								var angle = Mathf.Atan2(nCorY - nHeight / 2, nCorX - nWidth / 2) - Mathf.PI / 2;
								var checkAngle = Mathf.PI * 2 * objImage.fillAmount;
								angle = angle < 0 ? Mathf.PI * 2 + angle : angle;
								if (angle > checkAngle) { toExclude.Add(result); continue; }
							}
						}

						if (objImage.fillOrigin == (int)Image.Origin360.Left)
						{
							if (objImage.fillClockwise)
							{
								var angle = Mathf.Atan2(nCorY - nHeight / 2, nCorX - nWidth / 2) - Mathf.PI;
								var checkAngle = Mathf.PI * 2 * (1 - objImage.fillAmount);
								angle = angle < 0 ? Mathf.PI * 2 + angle : angle;
								if (angle < checkAngle) { toExclude.Add(result); continue; }
							}
							if (!objImage.fillClockwise)
							{
								var angle = Mathf.Atan2(nCorY - nHeight / 2, nCorX - nWidth / 2) - Mathf.PI;
								var checkAngle = Mathf.PI * 2 * objImage.fillAmount;
								angle = angle < 0 ? Mathf.PI * 2 + angle : angle;
								if (angle > checkAngle) { toExclude.Add(result); continue; }
							}
						}
					}
					#endregion

				}
				#endregion

				// Getting targeted pixel alpha from object's texture 
				float alpha = objTex.GetPixel((int)(texCorX + texRect.x), (int)(texCorY + texRect.y)).a;

				// Deciding if we need to exclude the object from results list
				if (objAlphaCheck)
				{
					if (objAlphaCheck.IncludeMaterialAlpha) alpha *= objImage.color.a;
					if (alpha < objAlphaCheck.AlphaThreshold) toExclude.Add(result);
				}
				else
				{
					if (IncludeMaterialAlpha) alpha *= objImage.color.a;
					if (alpha < AlphaThreshold) toExclude.Add(result);
				}
			}
			catch (UnityException e)
			{
				if (Application.isEditor && ShowTextureWarnings)
					Debug.LogWarning(string.Format("Check for alpha failed: {0}", e.Message));
			};
		}

		resultAppendList.RemoveAll(r => toExclude.Contains(r));
	}
}