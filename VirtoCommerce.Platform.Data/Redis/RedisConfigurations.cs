using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace VirtoCommerce.Platform.Data.Redis
{
    /// <summary>
    /// Manages redis client configurations for the cache handle.
    /// <para>
    /// Configurations will be added by the cache configuration builder/factory or the configuration
    /// loader. The cache handle will pick up the configuration matching the handle's name.
    /// </para>
    /// </summary>
    public static class RedisConfigurations
    {
        private static Dictionary<string, RedisConfiguration> _config = null;
        private static object _configLock = new object();

        private static Dictionary<string, RedisConfiguration> Configurations
        {
            get
            {
                if (_config == null)
                {
                    lock (_configLock)
                    {
                        if (_config == null)
                        {
                            _config = new Dictionary<string, RedisConfiguration>();
                        }
                    }
                }

                return _config;
            }
        }

        /// <summary>
        /// Adds the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">If configuration is null.</exception>
        public static void AddConfiguration(RedisConfiguration configuration)
        {
            lock (_configLock)
            {
                if (!Configurations.ContainsKey(configuration.Key))
                {
                    Configurations.Add(configuration.Key, configuration);
                }
            }
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <param name="configurationName">The identifier.</param>
        /// <returns>The <c>RedisConfiguration</c>.</returns>
        /// <exception cref="System.ArgumentNullException">If id is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// If no configuration was added for the id.
        /// </exception>
        public static RedisConfiguration GetConfiguration(IConfiguration configuration, string configurationName)
        {

            if (!Configurations.ContainsKey(configurationName))
            {
                // check connection strings if there is one matching the name
                var connectionString = configuration.GetConnectionString("configurationName");
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("No configuration added for configuration name " +
                                                        configurationName);
                }

                // defaulting to database 0, no way to set it via connection strings atm.
                var redisConfiguration = new RedisConfiguration(configurationName, connectionString);
                AddConfiguration(redisConfiguration);
            }

            return Configurations[configurationName];
        }
    }

}
