using Dynastream.Fit;

namespace Handlers.Fit;

public class FitDecoder
{
    public FitMessages FitMessages => _fitListener.FitMessages;

    public HashSet<string> RecordFieldNames = new HashSet<string>();
    public HashSet<string> RecordDeveloperFieldNames = new HashSet<string>();

    private FitListener _fitListener = new FitListener();
    private Stream inputStream;
    private Dynastream.Fit.File fileType;

    public FitDecoder(Stream stream, Dynastream.Fit.File fileType)
    {
        inputStream = stream;
        this.fileType = fileType;
    }

    public bool Decode()
    {
        // Create the Decode Object
        Decode decoder = new Decode();

        // Check that this is a FIT file
        if (!decoder.IsFIT(inputStream))
        {
            throw new Exception($"Expected FIT File Type: {fileType}, received a non FIT file.");
        }

        // Create the Message Broadcaster Object
        MesgBroadcaster messageBroadcaster = new MesgBroadcaster();

        // Connect the the Decode and Message Broadcaster Objects
        decoder.MesgEvent += messageBroadcaster.OnMesg;

        // Connect Message Broadcaster Events to their onMesg delegates
        messageBroadcaster.FileIdMesgEvent += (sender, e) =>
        {
            if ((e.mesg as FileIdMesg)?.GetType() != fileType)
            {
                throw new Exception($"Expected FIT File Type: {fileType}, received File Type: {(e.mesg as FileIdMesg)?.GetType()}");
            }
        };

        messageBroadcaster.RecordMesgEvent += (sender, e) =>
        {
            // Use LINQ for more modern field processing
            RecordFieldNames.UnionWith(
                e.mesg.Fields
                    .Where(field => !string.Equals(field.Name, "unknown", StringComparison.OrdinalIgnoreCase))
                    .Select(field => field.Name)
            );

            RecordDeveloperFieldNames.UnionWith(
                e.mesg.DeveloperFields.Select(devField => devField.Name)
            );
        };

        // Connect FitListener to get lists of each message type with FitMessages
        decoder.MesgEvent += _fitListener.OnMesg;

        // Decode the FIT File
        try
        {
            bool readOK = decoder.Read(inputStream);

            // If there are HR messages, merge the heart-rate data with the Record messages.
            if (readOK && FitMessages.HrMesgs.Count > 0)
            {
                HrToRecordMesgWithoutPlugin.MergeHeartRates(FitMessages);
                RecordFieldNames.Add("Heart Rate");
            }

            return readOK;
        }
        finally
        {
        }
    }
}
