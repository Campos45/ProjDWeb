﻿using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models;

// Modelo que representa um monumento
public class Monumento
{
    [Display(Name = "ID")]
    public int Id { get; set; }  // Identificador único do monumento

    [Display(Name = "Designação")]
    [Required]  // Campo obrigatório
    public string Designacao { get; set; }  // Nome ou designação do monumento

    [Display(Name = "Endereço")]
    public string Endereco { get; set; }  // Endereço do monumento (pode ser nulo)

    [Display(Name = "Coordenadas")]
    public string Coordenadas { get; set; }  // Coordenadas geográficas do monumento (latitude/longitude)

    [Display(Name = "Época de Construção")]
    public string EpocaConstrucao { get; set; }  // Período em que foi construído

    [Display(Name = "Descrição")]
    public string Descricao { get; set; }  // Descrição textual do monumento

    // Relação com o utilizador que criou o monumento (obrigatório)
    [Required(ErrorMessage = "O campo Utilizador é obrigatório.")]
    [Display(Name = "Utilizador")]
    public int UtilizadorId { get; set; }
    public Utilizador? Utilizador { get; set; }  // Propriedade de navegação para o utilizador

    // Relação com a localidade onde o monumento se situa (obrigatório)
    [Required(ErrorMessage = "O campo Localidade é obrigatório.")]
    [Display(Name = "Localidade")]
    public int LocalidadeId { get; set; }
    public Localidade? Localidade { get; set; }  // Propriedade de navegação para a localidade

    // Relação 1:N com imagens do monumento
    public ICollection<Imagem> Imagens { get; set; } = new List<Imagem>();

    // Relação N:N com características do monumento
    public ICollection<Caracteristicas> Caracteristicas { get; set; } = new List<Caracteristicas>();

    // Relação 1:N com visitas feitas ao monumento por utilizadores
    public List<VisitaMonumento> VisitasMonumento { get; set; } = new List<VisitaMonumento>();
}