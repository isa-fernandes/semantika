# semantika
App for helping language learning

## Instructions of use

If you generate the `.apk`, just install and open the app in your phone, select the desired language to translate to in the top-right dropdown menu, point yout phone to some object and click in the bottom-left button Screenshot, after a few seconds you should see the english name of the object and its translation to the chosen language being tracked :smile:

## Project Setup

1. Clone this repository in your computer and open it in [Unity](https://store.unity.com/#plans-individual)

### IMPORTANT!
I could not open the downloaded code directly in Unity, it crashes, so it is best to create a Unity project and copy the assets (including the scene!) into the new project and maybe importing Vuforia yourself before doing it

2. Open `Scripts/SceneManager.cs` and change line 29 to `string apiKey = "<YOUR_API_KEY>";`, you can obtain your API key by creating a project at [Google Cloud Platform](https://console.cloud.google.com/) and opening the menu `API and Services > Credentials > API Keys`

3. For iOS:
    1. Download [this file](https://drive.google.com/file/d/14a2_LlSMqQKWKuQWZw5bMVu6Nn6hSF19/view?usp=sharing) and save it inside the folder `Assets/Plugins/Grpc.Core/runtimes/ios/`
    2. Download [this file](https://drive.google.com/file/d/1rWUM9n-fr43ZKluFy95K8PvgT5XA-eZP/view?usp=sharing) and save it inside the folder `Plugins/Grpc.Core/runtimes/ios/`
  
4. For Korean font:
    1. Download [this asset](https://drive.google.com/file/d/1PZqUxCgpWL2jD-6JpyMAxSPqU6pqg69R/view?usp=sharing) and save it inside the folder `Assets/Resources/`
    2. Inside Unity, select the file `Assets/Resources/NotoSans-Regular SDF.asset` in unity project window and associate the above downloaded asset as a [fallback font](https://www.youtube.com/watch?v=pLW2B98W5AU&ab_channel=Zolran) in unity inspector window (as shown in the right side of the picture below)
    ![Picture showing Fallback Font Assets](https://i.ytimg.com/vi/pLW2B98W5AU/maxresdefault.jpg)
  
5. For other languages:
    1. Select the desired language at [NotoSans website](https://fonts.google.com/noto/fonts) and download the family
    2. Unzip it and move it to `Assets` folder where you want to _eg. Resources folder_
    3. Create a font asset using the obtained `Regular.otf` file (you can check the first section of [this tutorial](https://learn.unity.com/tutorial/textmesh-pro-font-asset-creation#) for font asset creation)
    4. While creating the font I suggest that you set `Sampling Point Size = 60`, `Padding = 5`, `Atlas Resolution = 2048x2048` and look at the [decimal range of the desired language](https://www.ssec.wisc.edu/~tomw/java/unicode.html) and set the `Custom Range` to it, but it can't be too big, so divide the range into many fonts with 1100 characters at most, for example the korean range is 44032-55203, it has more than 11.000 characters, so I repeated this process of creation and saved it as 11 different fonts, and set parts 2-11 as fallback fonts for part 1
    5. Then associate your font (or part 1 of the font) as fallback font for `NotoSans-Regular SDF` just like step 4.2
