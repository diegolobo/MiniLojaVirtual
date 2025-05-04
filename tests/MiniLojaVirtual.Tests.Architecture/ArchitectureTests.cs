using MiniLojaVirtual.Domain.Models.Users;
using MiniLojaVirtual.Infrastructure.Contexts;
using NetArchTest.Rules;

namespace MiniLojaVirtual.Tests.Architecture;

public class ArchitectureTests
{
	[Fact]
	public void InfrastructureMustDependOnDomain()
	{
		var result = Types.InAssembly(typeof(ApplicationDbContext).Assembly)
			.That()
			.ResideInNamespace("MiniLojaVirtual.Infraestrutura")
			.Should()
			.HaveDependencyOn("MiniLojaVirtual.Domain")
			.GetResult();

		Assert.True(result.IsSuccessful, "A camada de infraestrutura deve depender da camada de domínio.");
	}

	[Fact]
	public void InfrastructureMustNotDependOnWebApp()
	{
		var result = Types.InAssembly(typeof(ApplicationDbContext).Assembly)
			.That()
			.ResideInNamespace("MiniLojaVirtual.Infrastructure")
			.Should()
			.NotHaveDependencyOn("MiniLojaVirtual.Web")
			.GetResult();

		Assert.True(result.IsSuccessful,
			"A camada de infraestrutura não deve depender da camada de interface da aplicação web.");
	}

	[Fact]
	public void InfrastructureMustNotDependOnApi()
	{
		var result = Types.InAssembly(typeof(ApplicationDbContext).Assembly)
			.That()
			.ResideInNamespace("MiniLojaVirtual.Infrastructure")
			.Should()
			.NotHaveDependencyOn("MiniLojaVirtual.Api")
			.GetResult();

		Assert.True(result.IsSuccessful,
			"A camada de infraestrutura não deve depender da camada de interface da api.");
	}

	[Fact]
	public void DomainDoesNotDependOnInfrastructure()
	{
		var result = Types.InAssembly(typeof(ApplicationUser).Assembly)
			.That()
			.ResideInNamespace("MiniLojaVirtual.Domain")
			.Should()
			.NotHaveDependencyOn("MiniLojaVirtual.Infraestrutura")
			.GetResult();

		Assert.True(result.IsSuccessful, "A camada de domínio não deve depender da camada de infrastrutura.");
	}

	[Fact]
	public void DomainMustNotDependOnApplication()
	{
		var result = Types.InAssembly(typeof(ApplicationUser).Assembly)
			.That()
			.ResideInNamespace("MiniLojaVirtual.Domain")
			.Should()
			.NotHaveDependencyOn("MiniLojaVirtual.Application")
			.GetResult();

		Assert.True(result.IsSuccessful, "A camada de domínio não deve depender da camada de aplicação.");
	}

	[Fact]
	public void DomainMustNotDependOnWebApp()
	{
		var result = Types.InAssembly(typeof(ApplicationUser).Assembly)
			.That()
			.ResideInNamespace("MiniLojaVirtual.Domain")
			.Should()
			.NotHaveDependencyOn("MiniLojaVirtual.Web")
			.GetResult();

		Assert.True(result.IsSuccessful,
			"A camada de domínio não deve depender da camada de interface da aplicação web.");
	}

	[Fact]
	public void DomainMustNotDependOnApi()
	{
		var result = Types.InAssembly(typeof(ApplicationUser).Assembly)
			.That()
			.ResideInNamespace("MiniLojaVirtual.Domain")
			.Should()
			.NotHaveDependencyOn("MiniLojaVirtual.Api")
			.GetResult();

		Assert.True(result.IsSuccessful,
			"A camada de domínio não deve depender da camada de interface da api.");
	}

	[Fact]
	public void ApplicationMustDependOnDomain()
	{
		var result = Types.InAssembly(typeof(ApplicationUser).Assembly)
			.That()
			.ResideInNamespace("MiniLojaVirtual.Application")
			.Should()
			.HaveDependencyOn("MiniLojaVirtual.Domain")
			.GetResult();

		Assert.True(result.IsSuccessful, "A camada de aplicação deve depender da camada de domínio.");
	}

	[Fact]
	public void ApplicationMustNotDependOnInfrastructure()
	{
		var result = Types.InAssembly(typeof(ApplicationUser).Assembly)
			.That()
			.ResideInNamespace("MiniLojaVirtual.Application")
			.Should()
			.NotHaveDependencyOn("MiniLojaVirtual.Infraestrutura")
			.GetResult();

		Assert.True(result.IsSuccessful, "A camada de aplicação não deve depender da camada de infraestrutura.");
	}

	[Fact]
	public void ApplicationMustNotDependOnWebApp()
	{
		var result = Types.InAssembly(typeof(ApplicationUser).Assembly)
			.That()
			.ResideInNamespace("MiniLojaVirtual.Application")
			.Should()
			.NotHaveDependencyOn("MiniLojaVirtual.Web")
			.GetResult();

		Assert.True(result.IsSuccessful,
			"A camada de aplicação não deve depender da camada de interface da aplicação web.");
	}

	[Fact]
	public void ApplicationMustNotDependOnApi()
	{
		var result = Types.InAssembly(typeof(ApplicationUser).Assembly)
			.That()
			.ResideInNamespace("MiniLojaVirtual.Application")
			.Should()
			.NotHaveDependencyOn("MiniLojaVirtual.Api")
			.GetResult();

		Assert.True(result.IsSuccessful,
			"A camada de aplicação não deve depender da camada de interface da api.");
	}
}