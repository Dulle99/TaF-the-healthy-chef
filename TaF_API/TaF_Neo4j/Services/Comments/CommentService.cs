using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.Services.Comments
{
    public class CommentService : ICommentService
    {
        private IGraphClient _client;
        public CommentService(IGraphClient client)
        {
            this._client = client;
        }
    }
}
