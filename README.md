# Desafio C# Web API

## Descrição 
Criar uma Web API em C# com o banco de dados fornecido neste repositório, e desenvolver os seguintes end-points:

•	Obter a média de preço dos produtos das notas fiscais de uma empresa, arrecadadas em intervalo de datas “de-até” no formato (dd/MM/aaaa). O resultado deve ser agrupado por mês

•	Cadastrar nota fiscal com a seguinte regra de validação no servidor:<br />
    o	O valor total de uma nota fiscal deve ser a soma do preço dos seus produtos cadastrados na tabela NotasFiscaisProdutos

•	Atualizar uma nota fiscal com a seguinte regra de validação no servidor:<br />
    o	Somente é permitido atualizar a nota fiscal caso esta seja a última nota enviada pela empresa
    
•	Deletar uma nota fiscal

## Instruções e Avaliação

•   Para que avaliemos seu desafio, faça um fork deste repositório e execute um pull-request.

•   Descrever neste README do projeto os exemplos de uso da sua API

## Pontos avaliados:<br />
•	Consultas ao Banco de Dados <br />
•	Estrutura da Web API<br />
•	Qualidade do código<br />

Candidato: Jeremias dos Santos Francelino<br />
Exemplos de uso: <br />

• Obter a média de preço dos produtos das notas fiscais de uma empresa, arrecadadas em intervalo de datas “de-até” no formato       (dd/MM/aaaa). O resultado deve ser agrupado por mês:<br />
    URL: apimarqdesafio/NotasFiscais/?100=idEmpresa&101=dd/MM/yyyy&102=dd/MM/yyyy<br />
        100 -> IdEmpresa<br />
        101 -> Data inicial<br />
        102 -> Data final<br />
        Todos os campos de filtro são obrigatórios<br />
    Método: GET<br />
    Retorno:<br />
        [<br />
            {<br />
                'Mes': string,<br />
                'Produtos': [<br />
                    {<br />
                        'Nome': string,<br />
                        'Marca': string,<br />
                        'MediaPreco': decimal<br />
                    },...<br />
                ]<br />
            },...<br />
        ]<br />
• Cadastrar nota fiscal com a seguinte regra de validação no servidor:<br />
    ...<br />
• Atualizar uma nota fiscal com a seguinte regra de validação no servidor:<br />
    URL: apimarqdesafio/NotasFiscais<br />
        Enviar no body objeto (application/json) a ser atualizado no seguinte formato:<br />
            {<br />
                'Id': int,<br />
                'IdEmpresa': int,<br />
                'Total': decimal,<br />
                'DataHora': 'MM/dd/yyyy HH:mm:ss'<br />
            }<br />
    Método: PUT<br />
• Deletar uma nota fiscal<br />
    URL: apimarqdesafio/NotasFiscais/IdNotaFiscal<br />
    Método: DELETE<br />
