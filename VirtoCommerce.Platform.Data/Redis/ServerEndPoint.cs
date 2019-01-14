namespace VirtoCommerce.Platform.Data.Redis
{
    /// <summary>
    /// Defines an endpoint.
    /// </summary>
    public sealed class ServerEndPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerEndPoint"/> class.
        /// </summary>
        public ServerEndPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerEndPoint"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <exception cref="System.ArgumentNullException">If host is null.</exception>
        public ServerEndPoint(string host, int port)
        {
            Host = host;
            Port = port;
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        public string Host { get; set; }
    }
}
