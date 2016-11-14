﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundatio.Parsers.LuceneQueries.Nodes;

namespace Foundatio.Parsers.LuceneQueries.Visitors {
    public class QueryVisitorWithPriority : IChainableQueryVisitor {
        public int Priority { get; set; }
        public IQueryNodeVisitorWithResult<IQueryNode> Visitor { get; set; }

        public Task<IQueryNode> AcceptAsync(IQueryNode node, IQueryVisitorContext context) {
            return Visitor.AcceptAsync(node, context);
        }

        public Task VisitAsync(GroupNode node, IQueryVisitorContext context) {
            return Visitor.VisitAsync(node, context);
        }

        public Task VisitAsync(TermNode node, IQueryVisitorContext context) {
            return Visitor.VisitAsync(node, context);
        }

        public Task VisitAsync(TermRangeNode node, IQueryVisitorContext context) {
            return Visitor.VisitAsync(node, context);
        }

        public Task VisitAsync(ExistsNode node, IQueryVisitorContext context) {
            return Visitor.VisitAsync(node, context);
        }

        public Task VisitAsync(MissingNode node, IQueryVisitorContext context) {
            return Visitor.VisitAsync(node, context);
        }

        public class PriorityComparer : IComparer<QueryVisitorWithPriority> {
            public int Compare(QueryVisitorWithPriority x, QueryVisitorWithPriority y) {
                return x.Priority.CompareTo(y.Priority);
            }

            public static readonly PriorityComparer Instance = new PriorityComparer();
        }
    }
}
