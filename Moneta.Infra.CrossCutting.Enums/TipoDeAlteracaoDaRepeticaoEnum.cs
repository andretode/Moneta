﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Infra.CrossCutting.Enums
{
    public enum TipoDeAlteracaoDaRepeticaoEnum
    {
        AlterarApenasEste = 1,
        AlterarEsteESeguintes,
        AlterarTodos
    }
}
