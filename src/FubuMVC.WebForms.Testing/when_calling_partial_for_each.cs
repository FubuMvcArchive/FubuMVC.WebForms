using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.View;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.WebForms.Testing
{
    [TestFixture]
    public class when_calling_partial_for_each
    {
        private IFubuPage<InputModel> _page;
        private InputModel _model;
        private IPartialRenderer _renderer;
        private IPartialViewTypeRegistry _viewTypeRegistry;
        private IServiceLocator _serviceLocator;

        [SetUp]
        public void SetUp()
        {
            _page = MockRepository.GenerateMock<IFubuPage<InputModel>>();
            _renderer = MockRepository.GenerateStub<IPartialRenderer>();
            _serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
            
            _viewTypeRegistry = MockRepository.GenerateStub<IPartialViewTypeRegistry>();
            _serviceLocator.Stub(s => s.GetInstance<IPartialViewTypeRegistry>()).Return(_viewTypeRegistry);

            
            _model = new InputModel{Partials=new List<PartialModel>{new PartialModel()}};
            _page.Expect(p => p.Get<IElementGenerator<InputModel>>()).Return(MockRepository.GenerateMock<IElementGenerator<InputModel>>());;
            _page.Expect(p => p.Model).Return(_model);
            _page.Expect(p => p.Get<IPartialRenderer>()).Return(_renderer);
            _page.Expect(p => p.ServiceLocator).Return(_serviceLocator);
        }

        [Test]
        public void should_return_expression_without_calling_using()
        {
            _viewTypeRegistry.Stub(r => r.HasPartialViewTypeFor<PartialModel>()).Return(false);

            _page.PartialForEach(x=>x.Partials);
            
            _page.VerifyAllExpectations();
            _viewTypeRegistry.AssertWasCalled(r => r.HasPartialViewTypeFor<PartialModel>());
            _viewTypeRegistry.AssertWasNotCalled(r => r.GetPartialViewTypeFor<PartialModel>());
            _renderer.AssertWasNotCalled(r => r.CreateControl<object>(null, typeof(when_registering_partial_view_types.PartialView), null));
        }

        [Test]
        public void should_return_expression_after_calling_using()
        {
            _viewTypeRegistry.Stub(r => r.HasPartialViewTypeFor<PartialModel>()).Return(true);
            _viewTypeRegistry.Stub(r => r.GetPartialViewTypeFor<PartialModel>()).Return(typeof (when_registering_partial_view_types.PartialView));

            _page.PartialForEach(x => x.Partials).Using<when_registering_partial_view_types.PartialView>();

            _page.VerifyAllExpectations();
            _renderer.AssertWasCalled(r => r.CreateControl(_serviceLocator, typeof(when_registering_partial_view_types.PartialView), _model));
            _viewTypeRegistry.AssertWasCalled(r => r.HasPartialViewTypeFor<PartialModel>());
            _viewTypeRegistry.AssertWasCalled(r => r.GetPartialViewTypeFor<PartialModel>());
        }

        public class PartialModel { }
        public class InputModel { public IList<PartialModel> Partials { get; set; } }
    }
}