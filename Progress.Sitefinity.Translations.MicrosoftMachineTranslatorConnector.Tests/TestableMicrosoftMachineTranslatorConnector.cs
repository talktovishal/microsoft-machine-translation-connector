using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telerik.Sitefinity.Translations;

namespace Progress.Sitefinity.Translations.MicrosoftMachineTranslatorConnector.Tests
{
    internal class TestableMicrosoftMachineTranslatorConnector : MicrosoftMachineTranslatorConnector
    {
        public Func<HttpRequestMessage, HttpResponseMessage> mockedHttpClientSendAsyncDelegate { get; set; }
        public Func<HttpRequestMessage, HttpResponseMessage> mockedBreakSentenceHttpClientSendAsyncDelegate { get; set; }

        public bool MockedIsRemoveHtmlTagsEnabled { get; set; }

        public void InitializeCallMock(NameValueCollection config)
        {
            base.InitializeConnector(config);
        }

        public List<string> TranslateCallMock(List<string> input, ITranslationOptions translationOptions)
        {
            return base.Translate(input, translationOptions);
        }

        protected override HttpClient GetClient()
        {
            return new HttpClient(new MockedMessageHandler(this.mockedHttpClientSendAsyncDelegate));
        }

        protected override HttpClient GetBreakSentenceClient()
        {
            return new HttpClient(new MockedMessageHandler(this.mockedBreakSentenceHttpClientSendAsyncDelegate));
        }


        protected override bool IsRemoveHtmlTagsEnabled()
        {
            return this.MockedIsRemoveHtmlTagsEnabled;
        }

        private class MockedMessageHandler : HttpMessageHandler
        {
            public MockedMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> sendAsyncDelegate)
            {
                this.sendAsyncDelegate = sendAsyncDelegate;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (this.sendAsyncDelegate != null)
                {
                    return Task.FromResult(this.sendAsyncDelegate(request));
                }
                else
                {
                    return Task.FromResult(new HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.NotImplemented
                    });
                }
            }

            private readonly Func<HttpRequestMessage, HttpResponseMessage> sendAsyncDelegate;
        }
    }
}