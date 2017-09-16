﻿/*
Copyright (c) 2014, Lars Brubaker
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies,
either expressed or implied, of the FreeBSD Project.
*/
using System;
using System.Collections.Generic;
using MatterHackers.Agg.UI;
using MatterHackers.MeshVisualizer;
using MatterHackers.VectorMath;
using static MatterHackers.MatterControl.PrinterCommunication.PrinterConnection;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class PopupActionPanel : FlowLayoutWidget, IIgnoredPopupChild
	{
		public PopupActionPanel() : base(FlowDirection.TopToBottom)
		{
			this.Padding = 15;
			this.BackgroundColor = ActiveTheme.Instance.SecondaryBackgroundColor;
		}
	}

	public class MirrorControls : PopupActionPanel
	{
		private View3DWidget view3DWidget;

		private InteractiveScene scene;

		public MirrorControls(View3DWidget view3DWidget, InteractiveScene scene)
		{
			this.view3DWidget = view3DWidget;
			this.scene = scene;

			FlowLayoutWidget buttonContainer = new FlowLayoutWidget(FlowDirection.LeftToRight);
			buttonContainer.HAnchor = HAnchor.Fit;

			var theme = ApplicationController.Instance.Theme;

			Button mirrorXButton = theme.ButtonFactory.Generate("X");
			mirrorXButton.Margin = theme.ButtonSpacing;
			mirrorXButton.Click += (s, e) =>
			{
				if (scene.HasSelection)
				{
					scene.UndoBuffer.AddAndDo(new UndoRedoActions(() => MirrorOnAxis(Axis.X), () => MirrorOnAxis(Axis.X)));
				}
			};
			buttonContainer.AddChild(mirrorXButton);

			Button mirrorYButton = theme.ButtonFactory.Generate("Y");
			mirrorYButton.Margin = theme.ButtonSpacing;
			mirrorYButton.Click += (s, e) =>
			{
				if (scene.HasSelection)
				{
					scene.UndoBuffer.AddAndDo(new UndoRedoActions(() => MirrorOnAxis(Axis.Y), () => MirrorOnAxis(Axis.Y)));
				}
			};
			buttonContainer.AddChild(mirrorYButton);

			Button mirrorZButton = theme.ButtonFactory.Generate("Z");
			mirrorZButton.Margin = theme.ButtonSpacing;
			mirrorZButton.Click += (s, e) =>
			{
				if (scene.HasSelection)
				{
					scene.UndoBuffer.AddAndDo(new UndoRedoActions(() => MirrorOnAxis(Axis.Z), () => MirrorOnAxis(Axis.Z)));
				}
			};
			buttonContainer.AddChild(mirrorZButton);

			this.AddChild(buttonContainer);
		}

		private void MirrorOnAxis(Axis axis)
		{
			if (!scene.HasSelection)
			{
				return;
			}

			var selectedItem = scene.SelectedItem;
			selectedItem.Mesh.ReverseFaceEdges();

			switch (axis)
			{
				case Axis.Z:
					selectedItem.Matrix = PlatingHelper.ApplyAtCenter(selectedItem, Matrix4X4.CreateScale(1, 1, -1));

					break;
				case Axis.X:
					selectedItem.Matrix = PlatingHelper.ApplyAtCenter(selectedItem, Matrix4X4.CreateScale(-1, 1, 1));
					break;

				case Axis.Y:
					selectedItem.Matrix = PlatingHelper.ApplyAtCenter(selectedItem, Matrix4X4.CreateScale(1, -1, 1));
					break;
			}

			view3DWidget.PartHasBeenChanged();
			Invalidate();
		}
	}
}