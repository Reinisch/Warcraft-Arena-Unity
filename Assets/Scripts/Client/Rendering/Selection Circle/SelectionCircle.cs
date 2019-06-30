using Common;
using Core;
using UnityEngine;

namespace Client
{
    public partial class SelectionCircleController
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
                circleProjector.material = Object.Instantiate(circleProjector.material);
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
                if (circledUnit != null && controller.renderingReference.TryFind(circledUnit, out circledRenderer))
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
                circleProjector.gameObject.SetActive(true);
                circleProjector.transform.SetParent(attachedRenderer.transform, false);
                circleProjector.transform.position = circledRenderer.TagContainer.FindTag(settings.TargetTag);

                if (circledUnit.IsHostileTo(controller.player))
                    circleProjector.material.SetColor(MaterialColorProperty, settings.EnemyColor);
                else if (circledUnit.IsFriendlyTo(controller.player))
                    circleProjector.material.SetColor(MaterialColorProperty, settings.FriendlyColor);
                else
                    circleProjector.material.SetColor(MaterialColorProperty, settings.NeutralColor);

                circledRenderer = attachedRenderer;
            }

            private void Detach()
            {
                circleProjector.gameObject.SetActive(false);
                circleProjector.transform.SetParent(null, false);
                circledRenderer = null;
            }
        }
    }
}
