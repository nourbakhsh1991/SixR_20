   M S B A M L   `   `   `  ����B  >SixR_20, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:SixR_20SixR_20  5   	       SixR_20.Shell   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Z VFirstFloor.ModernUI, Version=1.0.9.0, Culture=neutral, PublicKeyToken=bc9b0c37bf06c6a90mui&http://firstfloorsoftware.com/ModernUI  5   	   Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5   	   U QMaterialDesignThemes.Wpf, Version=2.3.1.953, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	   !localclr-namespace:SixR_20  5   	   P LPrism.Wpf, Version=6.3.0.0, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59&regionshttp://prismlibrary.com/  5	   	   c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5
   	     � Title$  
MainWindow��5   	    � WindowStartupLocation$ CenterScreen=�5   	    � WindowState$ Normal=�5   	   $	��768��5   	   $
��1366��5   	   $��20q�5   	   $��14(�5   	   $��DemiBold"�5   	    � 
ResizeMode$ 	CanResize=�5   	      WindowBackground#��[  5   	   #��[  5   	    � WindowStyle$
 None=�5   	   .  (System.Windows.Interactivity.Interaction   Triggers 5      /  )System.Windows.Interactivity.EventTrigger  5   
      	EventName$ PreviewKeyDown��6   .  (System.Windows.Interactivity.TriggerBase   Actions. 5      6  0System.Windows.Interactivity.InvokeCommandAction     CommandParameter$
 true��6R   	   Command	 6$   �� 
 �� Path$
 keyDown� �� Mode$ OneWay=�6R   5        5   
   $ PreviewKeyUp��6   . 5        $ false��6P   	 6$   �� $
 keyUp�$ OneWay=�6P   5      .��5       �  � RowDefinitions 5!   
   �� 5"      $
��50px �6   �� 5#      $
��50px �6   �� 5$      $��2* �6   �� 5%       � ColumnDefinitions 5'   
   �� 5(      $��250px��6?   ��6   �� *LeftGridWidth+6?   �� 5)      $
��6.5* �6   �� 5*      $��3* �6   .+�5,   
   � -   5        $��TopGrid��5-      $��0��5,      $��0��5.      ��5/      �� *TopGridColspan+.+�50      �� -   5        $��HeaderRegion��51      ���#2cabe3!  Prism.Regions.RegionManager   
RegionName$ HeaderRegion��52        ItemControllStyle#��[ 53      6H   54      � 55   
   $��1��6   $��1��56      $��2��57       58      �� 59      $��* �6    �� 5:      $
��Auto �6    .+�5<      ��   ItemText#��[ 6   $��18(�5=      $��
20,10,20,0q�5>      $
��Bold"�5?      ��5@      �� *
HeaderText+66   �� 5A      $��1��6   $��
20,5,20,-5q�6%   5B      � 5D   
   -   5        $��LeftGrid��5F      $��1��5D      ���#2f323e$��0��5G      $��3��5H      .+�5I      �� -   5        $��
LeftRegion��5J      ���#2f323e$ 
LeftRegion��5K      #��[ 5L      6H   5M      � 5N   
   $��1��6   $��2��5O      ��5P      �� *MainRegionColspan+��5Q      �� *MainRegionRowspan+.+�5R      �� -   5        $��
MainRegion��5R       ���#2f323e#��[ 5U      $ 
MainRegion��5V      ��5T      �� *MainRegionThickness+5V      5W      � 5X   
   $��1��6   $��3��5Y      $��2��5Z      .+�5[      �� -   5        $��BottomRegion��5]      #��[ 5[      ���#2f323e$��
10,5,10,10q�5^      $ BottomRegion��5_      5`      � 5a   
   $��2��6   $��2��5b      .+�5c      �� -   5        $��RightRegion��5c      ���#2f323e#��[ 5e      $��	5,10,10,5q�5f      $ RightRegion��5g      5h      5k      5l      