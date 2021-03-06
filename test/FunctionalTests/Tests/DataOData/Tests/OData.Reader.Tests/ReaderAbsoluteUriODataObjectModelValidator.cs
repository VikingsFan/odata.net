﻿//---------------------------------------------------------------------
// <copyright file="ReaderAbsoluteUriODataObjectModelValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Atom;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Implementation of IODataObjectModelValidator which validates the all URIs returned from the reader on the OData OM are absolute.
    /// </summary>
    [ImplementationName(typeof(IODataObjectModelValidator), "ReaderAbsoluteUriODataObjectModelValidator")]
    public class ReaderAbsoluteUriODataObjectModelValidator : IODataObjectModelValidator
    {
        /// <summary>
        /// The assertion handler to use.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Validates the OData object model.
        /// </summary>
        /// <param name="objectModelRoot">The root of the object model.</param>
        public void ValidateODataObjectModel(object objectModelRoot)
        {
            ObjectModelVisitor visitor = new ObjectModelVisitor(this.Assert);
            visitor.Visit(objectModelRoot);
        }

        /// <summary>
        /// Visitor for the OData OM which performs the verification.
        /// </summary>
        private class ObjectModelVisitor : AtomMetadataODataObjectModelVisitor
        {
            /// <summary>
            /// The assertion handler to use.
            /// </summary>
            private AssertionHandler assert;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="assert">The assertion handler to use.</param>
            public ObjectModelVisitor(AssertionHandler assert)
            {
                this.assert = assert;
            }

            /// <summary>
            /// Visits a feed item.
            /// </summary>
            /// <param name="feed">The feed to visit.</param>
            protected override void VisitFeed(ODataFeed feed)
            {
                this.ValidateUri(feed.NextPageLink);
                base.VisitFeed(feed);
            }

            /// <summary>
            /// Visits an entry item.
            /// </summary>
            /// <param name="entry">The entry to visit.</param>
            protected override void VisitEntry(ODataEntry entry)
            {
                this.ValidateUri(entry.EditLink);
                this.ValidateUri(entry.ReadLink);
                base.VisitEntry(entry);
            }

            /// <summary>
            /// Visits a navigation link item.
            /// </summary>
            /// <param name="navigationLink">The navigation link to visit.</param>
            protected override void VisitNavigationLink(ODataNavigationLink navigationLink)
            {
                this.ValidateUri(navigationLink.Url);
                base.VisitNavigationLink(navigationLink);
            }

            /// <summary>
            /// Visits an ODataOperation.
            /// </summary>
            /// <param name="operation">The operation to visit.</param>
            protected override void VisitODataOperation(ODataOperation operation)
            {
                this.ValidateUri(operation.Target);
                base.VisitODataOperation(operation);
            }

            /// <summary>
            /// Visits a stream reference value (named stream).
            /// </summary>
            /// <param name="streamReferenceValue">The stream reference value to visit.</param>
            protected override void VisitStreamReferenceValue(ODataStreamReferenceValue streamReferenceValue)
            {
                this.ValidateUri(streamReferenceValue.EditLink);
                this.ValidateUri(streamReferenceValue.ReadLink);
                base.VisitStreamReferenceValue(streamReferenceValue);
            }

            /// <summary>
            /// Visits an entity reference link collection.
            /// </summary>
            /// <param name="entityReferenceLinks">The entity reference link collection to visit.</param>
            protected override void VisitEntityReferenceLinks(ODataEntityReferenceLinks entityReferenceLinks)
            {
                this.ValidateUri(entityReferenceLinks.NextPageLink);
                base.VisitEntityReferenceLinks(entityReferenceLinks);
            }

            /// <summary>
            /// Visits an entity reference link.
            /// </summary>
            /// <param name="entityReferenceLink">The entity reference link to visit.</param>
            protected override void VisitEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink)
            {
                this.ValidateUri(entityReferenceLink.Url);
                base.VisitEntityReferenceLink(entityReferenceLink);
            }

            /// <summary>
            /// Visits an ATOM feed metadata.
            /// </summary>
            /// <param name="atomFeedMetadata">The feed metadata to visit.</param>
            protected override void VisitAtomFeedMetadata(AtomFeedMetadata atomFeedMetadata)
            {
                this.ValidateUri(atomFeedMetadata.Icon);
                this.ValidateUri(atomFeedMetadata.Logo);
                base.VisitAtomFeedMetadata(atomFeedMetadata);
            }

            /// <summary>
            /// Visits an ATOM generator metadata.
            /// </summary>
            /// <param name="atomGeneratorMetadata">The generator metadata to visit.</param>
            protected override void VisitAtomGeneratorMetadata(AtomGeneratorMetadata atomGeneratorMetadata)
            {
                this.ValidateUri(atomGeneratorMetadata.Uri);
                base.VisitAtomGeneratorMetadata(atomGeneratorMetadata);
            }

            /// <summary>
            /// Visits an ATOM link metadata.
            /// </summary>
            /// <param name="atomLinkMetadata">The link metadata to visit.</param>
            protected override void VisitAtomLinkMetadata(AtomLinkMetadata atomLinkMetadata)
            {
                this.ValidateUri(atomLinkMetadata.Href);
                base.VisitAtomLinkMetadata(atomLinkMetadata);
            }

            /// <summary>
            /// Visits an ATOM person metadata.
            /// </summary>
            /// <param name="atomPersonMetadata">The person metadata to visit.</param>
            protected override void VisitAtomPersonMetadata(AtomPersonMetadata atomPersonMetadata)
            {
                this.ValidateUri(atomPersonMetadata.Uri);
                base.VisitAtomPersonMetadata(atomPersonMetadata);
            }

            /// <summary>
            /// Visits an ATOM categories metadata.
            /// </summary>
            /// <param name="atomCategoriesMetadata">The categories metadata to visit.</param>
            protected override void VisitAtomCategoriesMetadata(AtomCategoriesMetadata atomCategoriesMetadata)
            {
                this.ValidateUri(atomCategoriesMetadata.Href);
                base.VisitAtomCategoriesMetadata(atomCategoriesMetadata);
            }

            /// <summary>
            /// Validates the <paramref name="uri"/> that it's absolute.
            /// </summary>
            /// <param name="uri">The URI to validate.</param>
            private void ValidateUri(Uri uri)
            {
                if (uri != null)
                {
                    this.assert.IsTrue(uri.IsAbsoluteUri, "All Uri instances reported by readers must be absolute. Found a Uri '{0}' which is not.", uri.OriginalString);
                }
            }
        }
    }
}
