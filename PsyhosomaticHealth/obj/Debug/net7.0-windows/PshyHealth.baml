   M S B A M L   `   `   `  ����M  IPsyhosomaticHealth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:PsyhosomaticHealthPsyhosomaticHealth  5   	   #    PsyhosomaticHealth.PsyhHealth   5   	   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:PsyhosomaticHealth     5   	     � Title$  Расчет��5   	   $	��470��6   $	��820��6%   
������� � 
ResizeMode$ CanMinimize=�6D    � Icon$. (/vitruvianskij_chelovek_igevgex3djjh.ico��6^   .��5	      � .+�5
   
   o� .@�5      m� $��Файл��6   .@�5      m� -   5         m�Name$ newFile��5      $��
Новый��6,   6;   m� 5      -   5        $ openFile��5      $��Открыть��6-   6>   m� 5      -   5        $ saveFile��5      $��Сохранить��6-   ${�False��6@   m� 5      -   5        $ 
saveFileAs��5      $��Сохранить как��6/   ${�False��6F   m� 5      -   5        $ 	printFile��5      $��Печать��6.   ${�False��6>   m� 5      -   5        $ closeApp��5      $��
Выход��6-   5      m� 5      -   5        $ typesDiscipline��5      $%��Вид Деятельности��60   $
{�True��6J   .@�5      m� -   5        $ addDiscipline��5      $��Добавить��62   6D   m� 5      -	   5        $ changeDiscipline��5      $��Изменить��65   5      m� 5      -
   5        $
 help��5      $��Справка��6%   66   5      �� 5   
   -   5         ��Name$ setDiscipline��5      $��0,20,624,360q�6*   $%��Вид Деятельности��6@   $��14(�6Z   $��Italic$�6i   $y�Hidden=�6|   .��5      �� -   5         ��Name$ disciplineType��5       � ToolTip$6 0Выберите вид деятельности��60   $��27��6T   6`   5      �� 5   
   -   5        $ setDisciplineContent��5      $��0,80,624,295q�61   $��Вид Спорта��6G   $��14(�6[   $��Italic$�6j   $y�Hidden=�6}   .��5      �� -   5        $ disciplineTypeContent��5      $* $Выберите вид спорта��67   $��27��6U   5      �� 5   
   -   5         ��Name$ 	textBlock��5      $	��600��6'   $��200,20,20,74q�63   $��16(�6I   $
��Wrap=�6W   �� 5    
   -   5         ��Name$ 
stackPanel��5       $	��Top=�6)   $
��Left=�6A   $	��190��6\   $	��118��6h   $��	0,150,0,0q�6u   5!      �� 5"   
   -   5        	 ��Name$	 minSwitcher��5"      $	��100��6(   $��24,292,696,103q�64   $��Минуты��6L   $y�Hidden=�6]   $��14(�6q   �� 5#   
   -   5        
 ��Name$
 	getResult��5#      $��10,387,625,10q�6$   $��Расчитать��6;   ���#549438$��14(�6d   $y�Hidden=�6�    �� 	IsDefault$
 True��6�   �� 5$   
   -   5        $ 
colorBlock��5$      $��50��6(   $��57.04��63   $��221,387,549,10q�6B   ${�False��6Z   �� 5%   
   -   5        $ 
sportBlock��5%      $��291,398,256,0q�6(   $y�Visible=�6?   $	��Top=�6T   $��13(�6l   $
��Bold"�6z   5&      5'      