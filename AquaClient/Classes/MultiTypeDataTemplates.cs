using AquaClient.Classes.Responses;
using System.Windows.Input;


namespace AquaClient.Classes
{
    public class MultiTypeDataTemplateSelector: DataTemplateSelector
    {
        public DataTemplate ImageLabelSwitchTemplate {get; set;}
        public DataTemplate ImageLabelLabelTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var e = item as AquaDeviceInfo;
            if (e != null)
            {
                if (e.DeviceType == "switch")
                    return ImageLabelSwitchTemplate;
                else if (e.DeviceType == "sensor")
                    return ImageLabelLabelTemplate;
            };
            return ImageLabelLabelTemplate;
        }
    }

    public class ImageLabelSwitchItem: AquaDeviceInfo
    {
        public ICommand SwitchCommand { get; set; }
    }

    public class ImageLabelLabelItem : AquaDeviceInfo
    {   
        public ICommand SwitchCommand { get; set; }
    }

}
