using System;
using NRules.Extensibility;

namespace ServiceObserver.RuleEngine
{
	public class AspNetCoreDepResolver : IDependencyResolver
	{

		private IServiceProvider provider;

		public AspNetCoreDepResolver(IServiceProvider provider)
		{
			this.provider = provider;
		}

		public object Resolve(IResolutionContext context, Type serviceType)
		{
			return this.provider.GetService(serviceType);
		}
	}
}