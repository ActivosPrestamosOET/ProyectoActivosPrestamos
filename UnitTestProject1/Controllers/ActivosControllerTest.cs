using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace UnitTestProject1.Controllers
{
    [TestClass]
    public class ActivosControllerTest: SeleniumTest
    {
        public ActivosControllerTest() : base("Activos-PrestamosOET") { }

        [TestMethod]
        public void LoginTest()
        {
            this.ChromeDriver.Navigate().GoToUrl(this.GetAbsoluteUrl("/Account/Login"));
            this.ChromeDriver.FindElementById("Email").SendKeys("andresbejar@gmail.com");
            this.ChromeDriver.FindElementById("Password").SendKeys("Snakeeater@333");
            this.ChromeDriver.FindElementById("Email").Submit(); //submit form

            WebDriverWait wait = new WebDriverWait(this.ChromeDriver, TimeSpan.FromSeconds(10));
            Boolean funciono = wait.Until(ExpectedConditions.TitleContains("Inicio"));
            Assert.IsTrue(funciono);

        }

        public void LoginSetup()
        {
            this.ChromeDriver.Navigate().GoToUrl(this.GetAbsoluteUrl("/Account/Login"));
            this.ChromeDriver.FindElementById("Email").SendKeys("andresbejar@gmail.com");
            this.ChromeDriver.FindElementById("Password").SendKeys("Snakeeater@333");
            this.ChromeDriver.FindElementById("Email").Submit(); //submit form

            WebDriverWait wait = new WebDriverWait(this.ChromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.TitleContains("Inicio"));
        }

        [TestMethod]
        public void IngresarActivosTest()
        {

            LoginSetup();
            this.ChromeDriver.Navigate().GoToUrl(this.GetAbsoluteUrl("/Activos/Create"));
            this.ChromeDriver.FindElementById("DESCRIPCION").SendKeys("Prueba Selenium");
            this.ChromeDriver.FindElement(By.Id("PLACA")).SendKeys("333333333");
            this.ChromeDriver.FindElementById("NUMERO_SERIE").SendKeys("333333333");
            this.ChromeDriver.FindElementById("MODELO").SendKeys("WebDriver");
            this.ChromeDriver.FindElementById("FABRICANTE").SendKeys("Selenium");
            this.ChromeDriver.FindElementById("PRECIO").SendKeys("25000");
            this.ChromeDriver.FindElementById("NUMERO_DOCUMENTO").SendKeys("33333333");

            //proveedor
            IWebElement select = this.ChromeDriver.FindElementById("V_PROVEEDORIDPROVEEDOR");
            SelectElement selector = new SelectElement(select);
            selector.SelectByIndex(5); //seleccionamos A.Y.A

            //fecha compra
            IWebElement calendario = this.ChromeDriver.FindElementById("FECHA_COMPRA");
            //calendario.Click();
            calendario.SendKeys("037"); //3 de julio
            calendario.SendKeys(Keys.Tab);
            calendario.SendKeys("2016");

            //tipo transaccion
            IWebElement tipo_transaccion = this.ChromeDriver.FindElementById("TIPO_TRANSACCIONID");
            selector = new SelectElement(tipo_transaccion);
            selector.SelectByIndex(1); //Compra

            //Compañia
            IWebElement anfitriona = this.ChromeDriver.FindElementById("V_ANFITRIONAID");
            selector = new SelectElement(anfitriona);
            selector.SelectByIndex(1); //ESINTRO

            //Tipo Activo
            IWebElement tipo_activo = this.ChromeDriver.FindElementById("TIPO_ACTIVOID");
            selector = new SelectElement(tipo_activo);
            selector.SelectByIndex(1); //computadora

            //this.ChromeDriver.FindElementByCssSelector("input .btn .btn-default").Click(); //se envia la info
            this.ChromeDriver.FindElementById("DESCRIPCION").Submit();
            WebDriverWait wait = new WebDriverWait(this.ChromeDriver, TimeSpan.FromSeconds(10));
            Boolean funciono = wait.Until(ExpectedConditions.TitleContains("Index"));
            Assert.IsTrue(funciono);
             
        }
    }
}
