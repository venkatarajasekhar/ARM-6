﻿/*#############################################################################
 *    Copyright (C) 2006-2012 Mehanotronika RA                            
 *    All rights reserved.                                                     
 *	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 *                                                                             
 *	Описание: Интерфейс для унификации доступа к тегу (тегам) связанному с элементом интерфейса,
 *	            а также для фиксации факта изменения значения (e.g. для записи уставок)
 *                                                                             
 *	Файл                     : X:\Projects\33_Virica\Server_new_Interface\crza\CRZADevices\CRZADevices\CRZADeviceEv.cs                                         
 *	Тип конечного файла      :                                         
 *	версия ПО для разработки : С#, Framework 4.0                                
 *	Разработчик              : Юров В.И.                                        
 *	Дата начала разработки   : 07.02.2011 
 *	Дата посл. корр-ровки    : xx.хх.201х
 *	Дата (v1.0)              :                                                  
 ******************************************************************************
* Особенности реализации:
 * Используется ...
 *#############################################################################*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfaceLibrary
{
    public interface IHMITagAccess
    {
        /// <summary>
        /// Тег, связанный с данной 
        /// сущностью 
        /// </summary>
        ITag LinkedTag{get;}
        /// <summary>
        /// Теги, связанные с данной 
        /// сущностью 
        /// </summary>
        List<ITag> LinkedTags { get; }
        /// <summary>
        /// признак изменения
        /// значения связанного тега
        /// </summary>
        bool IsChange{get;set;}
        /// <summary>
        /// Имя тега в отображении
        /// </summary>
        string VisibleText { get; }
    }
}
