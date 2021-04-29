namespace SessionManagerExtension.Utils
{
    /// <summary>
    /// Provides a mechanism to serialize and deserialize JSON.
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// Serializes an object to a JSON string.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        string Serialize(object obj);

        /// <summary>
        /// Deserializes a JSON string to an object of the specified type.
        /// </summary>
        /// <typeparam name="T">Type to deserialize to.</typeparam>
        /// <param name="json">JSON string to deserialize.</param>
        /// <returns></returns>
        T Deserialize<T>(string json);
    }
}
