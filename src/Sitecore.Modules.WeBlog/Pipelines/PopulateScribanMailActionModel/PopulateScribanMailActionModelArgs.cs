using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Pipelines;
using Sitecore.Workflows.Simple;
using System;
using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel
{
    /// <summary>
    /// Pipeline arguments used in the weBlogPopulateScribanMailActionModel pipeline.
    /// </summary>
    public class PopulateScribanMailActionModelArgs : PipelineArgs
    {
        private Dictionary<string, object> _values = null;

        /// <summary>
        /// Gets the <see cref="WorkflowPipelineArgs"/> for the workflow pipeline which is running.
        /// </summary>
        public WorkflowPipelineArgs WorkflowPipelineArgs { get; }

        /// <summary>
        /// Gets or sets the entry item.
        /// </summary>
        public EntryItem EntryItem { get; set; }

        /// <summary>
        /// Gets or sets the comment item.
        /// </summary>
        public CommentItem CommentItem { get; set; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="workflowPipelineArgs">The <see cref="WorkflowPipelineArgs"/> for the workflow pipeline which is running.</param>
        public PopulateScribanMailActionModelArgs(WorkflowPipelineArgs workflowPipelineArgs)
        {
            if (workflowPipelineArgs == null)
                throw new ArgumentNullException(nameof(workflowPipelineArgs));

            WorkflowPipelineArgs = workflowPipelineArgs;

            _values = new Dictionary<string, object>();
        }

        /// <summary>
        /// Adds an object to the model.
        /// </summary>
        /// <param name="key">The key used to access the object.</param>
        /// <param name="value">The object to add.</param>
        public void AddModel(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException(nameof(key) + " cannot be whitespace only.", nameof(key));

            if (_values.ContainsKey(key))
                _values[key] = value;
            else
                _values.Add(key, value);
        }

        /// <summary>
        /// Remove an object from the model.
        /// </summary>
        /// <param name="key">The key used to access the object.</param>
        /// <returns>True if the object existed, otherwise false if the object didn't exist.</returns>
        public bool RemoveModel(string key)
        {
            return _values.Remove(key);
        }

        /// <summary>
        /// Checks to see whether an object is present in the model.
        /// </summary>
        /// <param name="key">The key used to access the object.</param>
        /// <returns>True if the object is present, otherwise false.</returns>
        public bool ModelContains(string key)
        {
            return _values.ContainsKey(key);
        }

        /// <summary>
        /// Gets an object from the model.
        /// </summary>
        /// <param name="key">The key used to access the object.</param>
        /// <returns>The object if present, otherwise null.</returns>
        public object GetModel(string key)
        {
            if (_values.TryGetValue(key, out object value))
                return value;

            return null;
        }

        /// <summary>
        /// Gets the entire model.
        /// </summary>
        /// <returns>The model.</returns>
        public Dictionary<string, object> GetModel()
        {
            return new Dictionary<string, object>(_values);
        }
    }
}