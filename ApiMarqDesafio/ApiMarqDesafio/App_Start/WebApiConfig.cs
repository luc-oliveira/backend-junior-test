using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ApiMarqDesafio
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Serviços e configuração da API da Web

            // Rotas da API da Web
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ApiMarqDesafio",
                routeTemplate: "apimarqdesafio/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
