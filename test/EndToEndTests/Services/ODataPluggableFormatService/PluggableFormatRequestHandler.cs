﻿//---------------------------------------------------------------------
// <copyright file="PluggableFormatRequestHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PluggableFormat
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Services.ODataWCFService;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;
    using Microsoft.Test.OData.Services.ODataWCFService.Handlers;

    class PluggableFormatRequestHandler : RootRequestHandler
    {
        public PluggableFormatRequestHandler(HttpMethod httpMethod, IODataDataSource dataSource)
            : base(httpMethod, dataSource) { }

        protected override RequestHandler DispatchHandler()
        {
            if (this.HttpMethod == HttpMethod.GET)
            {
                return new PluggableFormatQueryHandler(this);
            }
            else if (this.HttpMethod == HttpMethod.POST)
            {
                return new PluggableFormatCreateHandler(this);
            }

            return base.DispatchHandler();
        }

        public override Stream Process(Stream requestStream)
        {
            try
            {
                RequestHandler handler = this.DispatchHandler();

                if (this.PreferenceContext.RespondAsync)
                {
                    return handler.ProcessAsynchronously(requestStream);
                }

                return handler.Process(requestStream);
            }
            catch (Exception e)
            {
                ErrorHandler handler = new PluggableFormatErrorHandler(this, e);
                return handler.Process(null);
            }
        }
    }

    internal class PluggableFormatCreateHandler : CreateHandler
    {
        public PluggableFormatCreateHandler(RequestHandler other)
            : base(other) { }

        protected override RequestHandler DispatchHandler()
        {
            if (this.QueryContext.QueryPath.LastSegment is OperationSegment || this.QueryContext.QueryPath.LastSegment is OperationImportSegment)
            {
                return new PluggableFormatOperationHandler(this, HttpMethod.POST);
            }

            return base.DispatchHandler();
        }

        protected override ODataMessageWriterSettings GetWriterSettings()
        {
            var settings = base.GetWriterSettings();
            settings.MediaTypeResolver = PluggableFormatResolver.Instance;

            return settings;
        }
    }

    internal class PluggableFormatOperationHandler : OperationHandler
    {
        public PluggableFormatOperationHandler(RequestHandler other, HttpMethod httpMethod)
            : base(other, httpMethod)
        { }

        protected override ODataMessageReaderSettings GetReaderSettings()
        {
            var settings = base.GetReaderSettings();
            settings.MediaTypeResolver = PluggableFormatResolver.Instance;
            return settings;
        }
    }

    internal class PluggableFormatQueryHandler : QueryHandler
    {
        public PluggableFormatQueryHandler(RequestHandler other)
            : base(other) { }

        protected override ODataMessageWriterSettings GetWriterSettings()
        {
            var settings = base.GetWriterSettings();
            settings.MediaTypeResolver = PluggableFormatResolver.Instance;

            return settings;
        }
    }

    internal class PluggableFormatErrorHandler : ErrorHandler
    {
        public PluggableFormatErrorHandler(RequestHandler other, Exception exception)
            : base(other, exception)
        {
        }

        protected override ODataMessageWriterSettings GetWriterSettings()
        {
            var settings = base.GetWriterSettings();
            settings.MediaTypeResolver = PluggableFormatResolver.Instance;
            settings.SetContentType(string.IsNullOrEmpty(this.QueryContext.FormatOption) ? this.RequestAcceptHeader : this.QueryContext.FormatOption, Encoding.UTF8.WebName);
            return settings;
        }
    }
}
