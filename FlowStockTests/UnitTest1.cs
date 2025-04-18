using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading;

namespace FlowStockTests
{
    public class PruebasDeUsuario : IDisposable
    {
        private ChromeDriver driver;
        private string urlBase = "http://localhost:5095";

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            driver = new ChromeDriver(options);
        }

        [Test]
        public void RegistroLogin()
        {
            driver.Navigate().GoToUrl($"{urlBase}/Acceso/Registrate");

            driver.FindElement(By.Id("NombreUsuario")).SendKeys("edgardtest");
            driver.FindElement(By.Id("CorreoElectronico")).SendKeys("edgardtest@gmail.com");
            driver.FindElement(By.Id("Clave")).SendKeys("123456");
            driver.FindElement(By.Id("ConfirmarClave")).SendKeys("123456");

            driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Thread.Sleep(1000);

            if (driver.Url.Contains("/Acceso/Login"))
            {
                driver.FindElement(By.Id("CorreoElectronico")).SendKeys("edgardtest@gmail.com");
                driver.FindElement(By.Id("Clave")).SendKeys("123456");
                driver.FindElement(By.CssSelector("button[type='submit']")).Click();
                Thread.Sleep(1000);
            }

            Assert.That(driver.Url, Does.Contain("/Producto/Registro"));
        }

        [Test]
        public void AgregarProducto()
        {
            driver.Navigate().GoToUrl($"{urlBase}/Producto/Nuevo");

            driver.FindElement(By.Id("Nombre")).SendKeys("Helado");
            driver.FindElement(By.Id("Marca")).SendKeys("Bon");
            driver.FindElement(By.Id("Descripcion")).SendKeys("Producto agregado por automatizacion.");
            driver.FindElement(By.Id("Imagen")).SendKeys("https://heladosbon.com/wp-content/themes/heladosbon/images/helado.png");
            driver.FindElement(By.Id("CostoUnitario")).SendKeys("25.50");
            driver.FindElement(By.Id("Precio")).SendKeys("49.99");
            driver.FindElement(By.Id("StockActual")).SendKeys("20");
            driver.FindElement(By.Id("Categoria")).SendKeys("Postre");

            driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Thread.Sleep(1000);

            driver.Navigate().GoToUrl($"{urlBase}/Producto/Registro");
            Assert.That(driver.PageSource, Does.Contain("Helado"));
        }

        [Test]
        public void EditarProducto()
        {
            driver.Navigate().GoToUrl($"{urlBase}/Producto/Registro");

            var filas = driver.FindElements(By.CssSelector("table tbody tr"));
            var filaProducto = filas.FirstOrDefault(tr => tr.Text.Contains("Helado"));

            Assert.That(filaProducto, Is.Not.Null, "El producto 'Helado' no fue encontrado.");

            var botonEditar = filaProducto.FindElement(By.CssSelector("btn btn-warning"));
            botonEditar.Click();
            Thread.Sleep(1000);

            var campoNombre = driver.FindElement(By.Id("Nombre"));
            campoNombre.Clear();
            campoNombre.SendKeys("Producto Editado");

            var campoPrecio = driver.FindElement(By.Id("Precio"));
            campoPrecio.Clear();
            campoPrecio.SendKeys("59.99");

            driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Thread.Sleep(1000);

            driver.Navigate().GoToUrl($"{urlBase}/Producto/Registro");
            Assert.That(driver.PageSource, Does.Contain("Producto Editado"));
        }

        [Test]
        public void EliminarProducto()
        {
            driver.Navigate().GoToUrl($"{urlBase}/Account/Login");

            driver.FindElement(By.Id("Email")).SendKeys("usuario@test.com");
            driver.FindElement(By.Id("Password")).SendKeys("tu_contraseña");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Thread.Sleep(1000);

            driver.Navigate().GoToUrl($"{urlBase}/Producto/Registro");

            var filas = driver.FindElements(By.CssSelector("table tbody tr"));
            var filaProducto = filas.FirstOrDefault(tr => tr.Text.Contains("Producto Editado"));

            Assert.That(filaProducto, Is.Not.Null, "El producto no fue encontrado para eliminar.");

            var botonEliminar = filaProducto.FindElement(By.CssSelector("btn btn-danger"));
            botonEliminar.Click();
            Thread.Sleep(500);

            driver.SwitchTo().Alert().Accept();
            Thread.Sleep(1000);

            driver.Navigate().GoToUrl($"{urlBase}/Producto/Registro");
            Assert.That(driver.PageSource, Does.Not.Contain("Producto Editado"));
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        public void Dispose()
        {
            driver?.Dispose();
        }
    }
}
