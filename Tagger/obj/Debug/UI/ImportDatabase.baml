   M S B A M L   `   `   `  ����A  =Tagger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:Tagger.UI	Tagger.UI  5   	       Tagger.UI.ImportDatabase   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:Tagger.UI     5   	     � Title$  ImportDatabase��5   	   $	��300��6    $	��300��6-   .��5	      �  � RowDefinitions 5
   
   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   .+�5   
   �� $��Import File Location:��6   $��10,0,0,0q�60   $��Bottom=�6B   $��0��6]   � 5   
   $��1��6    � ColumnDefinitions 5      �� 5      $��4* �6#   �� 5      $��1* �6#   .+�5      �� -   5         ��Name$ FileLocationInput��5      $
��Left=�61   $��	10,10,0,0q�6L   $��0��6_    �� TextWrapping$
 Wrap=�6o   $	��Top=�6�   $	��210��6�   �� 5      -   5         ��Name$ OpenFileLocation��5      $��  ...  ��6/   $��1��6A   $��0,10,0,0q�6Q   $
��Left=�6c   $	��Top=�6~   6�   5      �� 5   
   $(��"Directory containing Tagged Files:��6   $��20,0,0,0q�6=   $��Bottom=�6O   $��2��6j   � 5   
   $��3��6    5      �� 5      $��4* �6#   �� 5      $��1* �6#   .+�5       �� -   5        $ ImageDirectoryInput��5       $
��Left=�63   $��	20,10,0,0q�6N   $
 Wrap=�6a   $	��Top=�6u   $	��200��6�   �� 5!      -   5        $ SelectImageDirectory��5!      $��  ...  ��63   $��1��6E   $��0,10,0,0q�6U   $
��Left=�6g   $	��Top=�6�   6�   5"      �� 5#   
   -   5        $ Import��5#      $��4��6!   $��Center=�6.   $��Bottom=�6K   $��0,0,0,10q�6f   $�� Import ��6x   6�   5&      5'      