using Common;
using Core;
using UnityEngine;

namespace Client
{
    public partial class RenderingReference
    {
        private partial class SelectionCircleController
        {
            private class SelectionCircle
            {
                private const string MaterialColorProperty = "_Color";

                private Unit circledUnit;
                private UnitRenderer circledRenderer;
                private Projector circleProjector;

                private readonly SelectionCircleController controller;
                private readonly SelectionCircleSettings settings;

                public SelectionCircle(SelectionCircleController controller, SelectionCircleSettings settings)
                {
                    this.controller = controller;
                    this.settings = settings;

                    circleProjector = GameObjectPool.Take(controller.selectionCirclePrototype);
                    circleProjector.material = Instantiate(circleProjector.material);
                    circleProjector.gameObject.SetActive(false);
                }

                public void Dispose()
                {
                    GameObjectPool.Return(circleProjector, false);

                    circledRenderer = null;
                    circledUnit = null;
                    circleProjector = null;
                }

                public void UpdateUnit(Unit newUnit)
                {
                    if (circledRenderer != null && circledUnit != newUnit)
                        Detach();

                    circledUnit = newUnit;
                    if (circledUnit != null && controller.rendering.unitRendererController.TryFind(circledUnit, out circledRenderer))
                        HandleRendererAttach(circledRenderer);
                }

                public void HandleRendererAttach(UnitRenderer attachedRenderer)
                {
                    if (attachedRenderer.Unit == circledUnit)
                        Attach(attachedRenderer);
                }

                public void HandleRendererDetach(UnitRenderer detachedRenderer)
                {
                    if (circledRenderer == detachedRenderer)
                        Detach();
                }

                private void Attach(UnitRenderer attachedRenderer)
                {
                    circledRenderer = attachedRenderer;
                    circleProjector.transform.SetParent(attachedRenderer.transform, false);
                    circleProjector.transform.position = circledRenderer.TagContainer.FindTag(settings.TargetTag);
                    circleProjector.gameObject.SetActive(true);

                    if (circledUnit.IsHostileTo(controller.rendering.Player))
                        circleProjector.material.SetColor(MaterialColorProperty, settings.EnemyColor);
                    else if (circledUnit.IsFriendlyTo(controller.rendering.Player))
                        circleProjector.material.SetColor(MaterialColorProperty, settings.FriendlyColor);
                    else
                        circleProjector.material.SetColor(MaterialColorProperty, settings.NeutralColor);
                }

                private void Detach()
                {
                    circleProjector.gameObject.SetActive(false);
                    circleProjector.transform.SetParent(null, false);
                    circledRenderer = null;
                    circledUnit = null;
                }
            }
        }
    }
}
